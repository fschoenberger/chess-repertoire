using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using DynamicData;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace ChessRepertoire.Model.Board;

public record BoardState(Dictionary<Square, ChessPiece> Pieces, Square? EnPassantTargetSquare, CastlingRights CastlingRights, Color CurrentTurn) {
    private const int BoardSize = 8;
    private const int NumPieces = 12; // 6 piece types in 2 colors
    private const int NumSquares = BoardSize * BoardSize;

    private readonly ILogger _logger = new DebugLoggerProvider().CreateLogger(nameof(BoardState));

    //private readonly ulong _id = GetZobristHash();
    public ulong Id => GetZobristHash();
    
    private ulong GetZobristHash() {
        ulong hash = 0;

        foreach (var piece in Pieces.Values) {
            var index = (int)piece.Type + (piece.Color == Color.White ? 6 : 0);
            hash ^= ZobristConstants.PieceKeys[index * NumSquares + piece.Square.Rank * 8 + piece.Square.File];
        }

        if (CurrentTurn != Color.White) {
            hash ^= ZobristConstants.SideKey;
        }

        if ((CastlingRights & CastlingRights.BlackKingSide) != 0) {
            hash ^= ZobristConstants.CastleKeys[0];
        }

        if ((CastlingRights & CastlingRights.BlackQueenSide) != 0) {
            hash ^= ZobristConstants.CastleKeys[1];
        }

        if ((CastlingRights & CastlingRights.WhiteKingSide) != 0) {
            hash ^= ZobristConstants.CastleKeys[2];
        }

        if ((CastlingRights & CastlingRights.WhiteQueenSide) != 0) {
            hash ^= ZobristConstants.CastleKeys[3];
        }

        if (EnPassantTargetSquare != null) {
            hash ^= ZobristConstants.EnPassantKeys[EnPassantTargetSquare.File];
        }

        return hash;
    }

    // Currently, this is a pseudo-FEN, in the future this should probably be replaced with Zobrist hashing.
    // https://en.wikipedia.org/wiki/Zobrist_hashing
    private string GetFen() {
        var builder = new StringBuilder();

        var emptySquares = 0;

        for (var rank = 7; rank >= 0; rank--) {
            for (var file = 0; file < 8; file++) {
                var square = new Square(file, rank);
                var found = Pieces.TryGetValue(square, out var piece);

                if (!found) {
                    emptySquares++;
                } else {
                    if (emptySquares > 0) {
                        builder.Append(emptySquares);
                        emptySquares = 0;
                    }

                    builder.Append(piece!.ToFenSymbol());
                }
            }

            if (emptySquares > 0) {
                builder.Append(emptySquares);
                emptySquares = 0;
            }

            if (rank > 0) {
                builder.Append('/');
            }
        }


        return builder.ToString();
    }

    public BoardState MakeMove(Move? move) {

        #region Validation
        if (IsCheckmate()) {
            _logger.LogDebug("Move is not legal because the game has already ended.");
            throw new InvalidOperationException("Move is not legal because the game has already ended.");
        }

        if (move == null) {
            _logger.LogDebug("Move is not legal because it was null.");
            throw new InvalidOperationException("Move is not legal because the game has already ended.");
        }

        if (!move.From.IsValid() || !move.To.IsValid()) {
            _logger.LogDebug("Move is not legal because either from or two are not valid squares.");
            throw new InvalidOperationException("Move is not legal because the game has already ended.");
        }

        if (move.From == move.To) {
            _logger.LogDebug("Move is illegal because it is an empty move.");
            throw new InvalidOperationException("Move is not legal because the game has already ended.");
        }

        if (!Pieces.TryGetValue(move.From, out var piece)) {
            _logger.LogDebug("Move is not legal because there is no piece on the from square.");
            throw new InvalidOperationException("Move is not legal because the game has already ended.");
        }

        if (piece.Color != CurrentTurn) {
            _logger.LogDebug("Move is not legal because the piece on the from square has the wrong color.");
            throw new InvalidOperationException("Move is not legal because the game has already ended.");
        }
        #endregion

        return IsCastlingMove(piece, move) ? TryCastling(piece, move) : TryMove(piece, move);
    }

    private BoardState TryMove(ChessPiece piece, Move move) {
        if (!CanMoveTo(piece, move.To, Pieces)) {
            _logger.LogDebug("Move is not legal because the piece can't move there.");
            throw new InvalidOperationException("Move is not legal because the piece can't move there.");
        }

        if (!UnobstructedPathExists(move.From, move.To, Pieces)) {
            _logger.LogDebug("Move is not legal because the path is obstructed.");
            throw new InvalidOperationException("Move is not legal because the path is obstructed.");
        }

        var king = Pieces.Values.First(p => p.Color == CurrentTurn && p.Type == PieceType.King);
        var kingSquare = king.Square == move.From ? move.To : king.Square;

        // Figure out if moving this piece would put the king in check or if the king currently is in check
        // First, we make the move and get all pieces. Next, we check if there are any attackers on the king.
        var pieces = Pieces.ToDictionary(p => p.Key, p => p.Value);
        pieces.Remove(move.From);
        pieces[move.To] = piece with { Square = move.To };

        var checkingPieces = GetAttackers(kingSquare, CurrentTurn.Flipped(), pieces).ToList();
        if (checkingPieces.Any()) {
            var str = checkingPieces.Aggregate("", (acc, p) => acc + p.ToString() + ", ");
            _logger.LogDebug("Move is not legal because [{str}] would be putting the {CurrentTurn} king in check.", str, CurrentTurn);
            throw new InvalidOperationException($"Move is not legal because {str} would be putting the {CurrentTurn} king in check.");
        }

        if (piece.Type == PieceType.Pawn && move.To.Rank is 0 or 7) // Pawns only move forward so no need to check for color
        {
            Debug.Assert(move.PromotedPiece != null);
            pieces[move.To] = move.PromotedPiece with { Square = move.To };
        } else {
            pieces[move.To] = piece with { Square = move.To };
        }

        if (move.To == EnPassantTargetSquare) {
            pieces.Remove(move.To with { Rank = move.From.Rank });
        }

        _logger.LogDebug("Moved {} to {}.", piece, move.To);

        Square? enPassantTargetSquare = null;
        if (piece.Type == PieceType.Pawn && Math.Abs(move.From.Rank - move.To.Rank) == 2) {
            enPassantTargetSquare = move.From with { Rank = move.From.Rank + (CurrentTurn == Color.White ? 1 : -1) };
        }

        Pieces.TryGetValue(move.From, out var capturedPiece);
        return new BoardState(pieces, enPassantTargetSquare, CalculateCastlingRights(move, piece, capturedPiece, CastlingRights), CurrentTurn.Flipped());
    }

    private BoardState TryCastling(ChessPiece piece, Move move) {
        // There probably are nicer ways to write this, buuut... ¯\_(ツ)_/¯
        var direction = (piece, move.From, move.To) switch {
            ( { Color: Color.White, Type: PieceType.King }, { File: 4, Rank: 0 }, { File: 2, Rank: 0 }) => CastlingRights.WhiteQueenSide,
            ( { Color: Color.White, Type: PieceType.King }, { File: 4, Rank: 0 }, { File: 6, Rank: 0 }) => CastlingRights.WhiteKingSide,
            ( { Color: Color.Black, Type: PieceType.King }, { File: 4, Rank: 7 }, { File: 2, Rank: 7 }) => CastlingRights.BlackQueenSide,
            ( { Color: Color.Black, Type: PieceType.King }, { File: 4, Rank: 7 }, { File: 6, Rank: 7 }) => CastlingRights.BlackKingSide,
            _ => throw new Exception("This should never happen!")
        };

        if ((CastlingRights & direction) == 0) {
            _logger.LogDebug("May not castle {}!", direction);
            throw new InvalidOperationException($"May not castle {direction}!");
        }

        var squares = direction switch {
            CastlingRights.WhiteQueenSide => new List<Square> {
                new(2, 0), //c1
                new(3, 0), //d1
                new(4, 0), //e1, where the King is
            },
            CastlingRights.BlackQueenSide => new List<Square>() {
                new(2, 7), //c7
                new (3, 7), //d7
                new (4, 7), //e7, where the King is
            },
            CastlingRights.WhiteKingSide => new List<Square>()
            {
                new (4, 0), //e1, where the King is
                new(5, 0), //f1
                new(6, 0), //g1
            },
            CastlingRights.BlackKingSide => new List<Square>()
            {
                new (4, 7), //e7, where the King is
                new(5, 7), //f7
                new(6, 7), //g7
            },
            _ => new List<Square>(),
        };

        if (!UnobstructedPathExists(move.From, move.To, Pieces)) {
            _logger.LogDebug("Castling is not legal because the path is obstructed.");
            throw new InvalidOperationException("Castling is not legal because the path is obstructed.");
        }

        if (squares.Any(v => GetAttackers(v, CurrentTurn.Flipped(), Pieces).Any())) {
            _logger.LogDebug("Can't castle because castling would require moving through check.");
            throw new InvalidOperationException("Castling is not legal because the path is obstructed.");
        }

        var pieces = Pieces.ToDictionary(entry => entry.Key, entry => entry.Value);

        pieces.Remove(move.From);
        pieces.Add(move.To, piece with { Square = move.To });

        var castlingRights = CastlingRights;

        // We know it's a legal move so no validation necessary
        switch (direction) {
            case CastlingRights.WhiteKingSide:
                pieces.Remove(new Square(7, 0));
                pieces.Add(new Square(5, 0), new ChessPiece(Color.White, PieceType.Rook, new Square(5, 0)));
                castlingRights &= ~(CastlingRights.WhiteKingSide | CastlingRights.WhiteQueenSide);
                break;

            case CastlingRights.WhiteQueenSide:
                pieces.Remove(new Square(0, 0));
                pieces.Add(new Square(3, 0), new ChessPiece(Color.White, PieceType.Rook, new Square(3, 0)));
                castlingRights &= ~(CastlingRights.WhiteKingSide | CastlingRights.WhiteQueenSide);
                break;

            case CastlingRights.BlackKingSide:
                pieces.Remove(new Square(7, 7));
                pieces.Add(new Square(5, 7), new ChessPiece(Color.Black, PieceType.Rook, new Square(5, 7)));
                castlingRights &= ~(CastlingRights.BlackKingSide | CastlingRights.BlackQueenSide);
                break;

            case CastlingRights.BlackQueenSide:
                pieces.Remove(new Square(0, 7));
                pieces.Add(new Square(3, 7), new ChessPiece(Color.Black, PieceType.Rook, new Square(3, 7)));
                castlingRights &= ~(CastlingRights.BlackKingSide | CastlingRights.BlackQueenSide);
                break;
            default: throw new Exception("This should never happen!");
        }

        return new BoardState(
            pieces,
            null,
            castlingRights,
            CurrentTurn.Flipped()
        );
    }

    private static bool IsCastlingMove(ChessPiece piece, Move move) {
        return (piece, move.From, move.To) switch {
            ( { Color: Color.White, Type: PieceType.King }, { File: 4, Rank: 0 }, { File: 2, Rank: 0 }) => true,
            ( { Color: Color.White, Type: PieceType.King }, { File: 4, Rank: 0 }, { File: 6, Rank: 0 }) => true,
            ( { Color: Color.Black, Type: PieceType.King }, { File: 4, Rank: 7 }, { File: 2, Rank: 7 }) => true,
            ( { Color: Color.Black, Type: PieceType.King }, { File: 4, Rank: 7 }, { File: 6, Rank: 7 }) => true,
            _ => false,
        };
    }

    private CastlingRights CalculateCastlingRights(Move move, ChessPiece piece, ChessPiece? capturedPiece, CastlingRights oldCastlingRights) {
        var castlingRights = oldCastlingRights;

        if (piece.Type == PieceType.King && !IsCastlingMove(piece, move)) {
            switch (piece.Color) {
                case Color.White:
                    if ((castlingRights & (CastlingRights.WhiteKingSide | CastlingRights.WhiteQueenSide)) != 0) {
                        castlingRights &= ~(CastlingRights.WhiteKingSide | CastlingRights.WhiteQueenSide);
                        _logger.LogDebug("White lost all castling rights because they moved their king.");
                    }

                    break;
                case Color.Black:
                    if ((castlingRights & (CastlingRights.BlackKingSide | CastlingRights.BlackQueenSide)) != 0) {
                        castlingRights &= ~(CastlingRights.BlackKingSide | CastlingRights.BlackQueenSide);
                        _logger.LogDebug("White lost all castling rights because they moved their king.");
                    }

                    break;
            }
        }

        if (piece.Type == PieceType.Rook) {
            switch (move.From) {
                case { File: 0, Rank: 0 }:
                    if ((castlingRights & CastlingRights.WhiteQueenSide) != 0) {
                        castlingRights &= ~CastlingRights.WhiteQueenSide;
                        _logger.LogDebug("White lost queenside castling rights because they moved their rook from the starting position.");
                    }

                    break;
                case { File: 7, Rank: 0 }:
                    if ((castlingRights & CastlingRights.WhiteKingSide) != 0) {
                        castlingRights &= ~CastlingRights.WhiteKingSide;
                        _logger.LogDebug("White lost kingside castling rights because they moved their rook from the starting position.");
                    }

                    break;
                case { File: 0, Rank: 7 }:
                    if ((castlingRights & CastlingRights.BlackQueenSide) != 0) {
                        castlingRights &= ~CastlingRights.BlackQueenSide;
                        _logger.LogDebug("Black lost queenside castling rights because they moved their rook from the starting position.");
                    }

                    break;
                case { File: 7, Rank: 7 }:
                    if ((castlingRights & CastlingRights.BlackKingSide) != 0) {
                        castlingRights &= ~CastlingRights.BlackKingSide;
                        _logger.LogDebug("Black lost kingside castling rights because they moved their rook from the starting position.");
                    }

                    break;
            }
        }

        if (capturedPiece is not { Type: PieceType.Rook })
            return castlingRights;

        switch (move.To) {
            case { File: 0, Rank: 0 }:
                if ((castlingRights & CastlingRights.WhiteQueenSide) != 0) {
                    castlingRights &= ~CastlingRights.WhiteQueenSide;
                    _logger.LogDebug("White lost queenside castling rights because their rook was captured.");
                }

                break;
            case { File: 7, Rank: 0 }:
                if ((castlingRights & CastlingRights.WhiteKingSide) != 0) {
                    castlingRights &= ~CastlingRights.WhiteKingSide;
                    _logger.LogDebug("White lost kingside castling rights because their rook was captured.");
                }

                break;
            case { File: 0, Rank: 7 }:
                if ((castlingRights & CastlingRights.BlackQueenSide) != 0) {
                    castlingRights &= ~CastlingRights.BlackQueenSide;
                    _logger.LogDebug("White lost queenside castling rights because their rook was captured.");
                }

                break;
            case { File: 7, Rank: 7 }:
                if ((castlingRights & CastlingRights.BlackKingSide) != 0) {
                    castlingRights &= ~CastlingRights.BlackKingSide;
                    _logger.LogDebug("White lost kingside castling rights because their rook was captured.");
                }

                break;
        }

        return castlingRights;
    }

    private bool CanMoveTo(ChessPiece piece, Square target, IDictionary<Square, ChessPiece> pieces) {
        if (!piece.CanMoveTo(target))
            return false;

        if (piece.Type == PieceType.Pawn) {
            if (target.File != piece.Square.File) {
                // Pawns may only capture diagonally, so we check if there's something to capture
                return target == EnPassantTargetSquare || pieces.ContainsKey(target);
            }

            // Pawns can't capture pieces in front of them, so we if they move forward there can't be anything in the way
            return !pieces.ContainsKey(target);

        }


        return (piece.Type == PieceType.Knight || UnobstructedPathExists(piece.Square, target, pieces));
    }

    /// <summary>
    /// This method checks if there is a path between two points first and second that can be taken with a chess piece. It assumes both first and second are empty.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <param name="pieces"></param>
    /// <returns>True if there is no piece in-between, false otherwise.</returns>
    private static bool UnobstructedPathExists(Square first, Square second, IDictionary<Square, ChessPiece> pieces) {
        if (!first.IsValid() || !second.IsValid() || first == second)
            return false;

        var direction = new Vector2(first.File, first.Rank) - new Vector2(second.File, second.Rank);

        var diagonally = Math.Abs((int)direction.X) == Math.Abs((int)direction.Y);
        var horizontally = direction.Y == 0;
        var vertically = direction.X == 0;
        var knight = Math.Abs((int)direction.X) == 2 && Math.Abs((int)direction.Y) == 1
                     || Math.Abs((int)direction.X) == 1 && Math.Abs((int)direction.Y) == 2;

        if (!(diagonally || horizontally || vertically)) {
            // Knights are always unobstructed
            return knight;
        }

        if (vertically) {
            Debug.Assert(first.File == second.File);

            var start = Math.Min(first.Rank, second.Rank) + 1;
            var end = Math.Max(first.Rank, second.Rank) - 1;

            for (var i = start; i <= end; ++i) {
                if (pieces.ContainsKey(new Square(first.File, i))) {
                    return false;
                }
            }

            return true;
        } else if (horizontally) {
            Debug.Assert(first.Rank == second.Rank);

            var start = Math.Min(first.File, second.File) + 1;
            var end = Math.Max(first.File, second.File) - 1;

            for (var i = start; i <= end; ++i) {
                if (pieces.ContainsKey(new Square(i, first.Rank))) {
                    return false;
                }
            }

            return true;
        } else if (diagonally) {
            // We know that the direction is not horizontal or vertical, so it must be diagonally
            var stepX = direction.X > 0 ? -1 : 1;
            var stepY = direction.Y > 0 ? -1 : 1;

            for (var i = 1; i < Math.Abs(direction.X); ++i) {
                var square = new Square(first.File + i * stepX, first.Rank + i * stepY);

                if (pieces.TryGetValue(square, out var piece)) {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    private IEnumerable<ChessPiece> GetAttackers(Square square, Color color, IDictionary<Square, ChessPiece> pieces) {
        return pieces.Values.Where(p => p.Color == color && CanMoveTo(p, square, pieces));
    }

    private bool IsLegalMoveHack(Move? move) {
        try {
            MakeMove(move);
            return true;
        }
        catch {
            return false;
        }
    }

    public IEnumerable<Move> GetLegalMoves() {
        return Pieces.Values
            .Where(p => p.Color == CurrentTurn)
            .SelectMany(p => p.GetPotentiallyLegalMoves())
            .Where(IsLegalMoveHack);
    }

    public bool IsCheckmate() {
        return false;
        //return GetLegalMoves().Any();
    }
}
