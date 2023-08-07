using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using DynamicData;
using DynamicData.Cache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ChessRepertoire.Model.Board;

[Flags]
public enum CastlingRights
{
    None = 0,
    WhiteKingSide = 1,
    WhiteQueenSide = 2,
    BlackKingSide = 4,
    BlackQueenSide = 8
}

public class ChessBoard : ReactiveObject
{
    private Square? _enPassantTargetSquare;
    private CastlingRights _castlingRights;

    private readonly Stack<Move?> _moves = new();

    public static string InitialPositionFen => "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    [Reactive]
    public Color CurrentTurn { get; private set; }

    [Reactive]
    public int HalfMoveClock { get; private set; }

    [Reactive]
    public int FullMoveNumber { get; private set; }

    public readonly SourceCache<ChessPiece, Square> Pieces = new(p => p.Square);

    private readonly ILogger? _logger;

    public Square? EnPassantTargetSquare => _enPassantTargetSquare;

    public CastlingRights CastlingRights => _castlingRights;

    public ICommand MoveCommand { get; }

    public ChessBoard(
        IEnumerable<ChessPiece> pieces,
        Color currentTurn = Color.White,
        int halfMoveClock = 0,
        int fullMoveNumber = 1,
        CastlingRights castlingRights = CastlingRights.BlackKingSide | CastlingRights.BlackQueenSide | CastlingRights.WhiteKingSide | CastlingRights.WhiteQueenSide,
        Square? enPassantTargetSquare = null,
        ILogger<ChessBoard>? logger = null)
    {
        _logger = logger ?? new DebugLoggerProvider().CreateLogger(nameof(ChessBoard));

        MoveCommand = ReactiveCommand.Create<Move, bool>(MakeMove);

        CurrentTurn = currentTurn;
        HalfMoveClock = halfMoveClock;
        FullMoveNumber = fullMoveNumber;
        _castlingRights = castlingRights;
        _enPassantTargetSquare = enPassantTargetSquare;

        Pieces.Edit(c =>
        {
            foreach (var piece in pieces)
            {
                c.AddOrUpdate(piece);
            }
        });
    }

    public bool CanMoveTo(ChessPiece piece, Square target)
    {
        return CanMoveTo(piece, target, Pieces.Items.ToDictionary(x => x.Square));
    }

    private bool CanMoveTo(ChessPiece piece, Square target, IDictionary<Square, ChessPiece> pieces)
    {
        if (!piece.CanMoveTo(target))
            return false;

        if (piece.Type == PieceType.Pawn)
        {
            if (target.File != piece.Square.File)
            {
                // Pawns may only capture diagonally, so we check if there's something to capture
                return pieces.ContainsKey(target);
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
    private bool UnobstructedPathExists(Square first, Square second, IDictionary<Square, ChessPiece> pieces)
    {
        if (!first.IsValid() || !second.IsValid() || first == second)
            return false;

        var direction = new Vector2(first.File, first.Rank) - new Vector2(second.File, second.Rank);

        var diagonally = Math.Abs((int)direction.X) == Math.Abs((int)direction.Y);
        var horizontally = direction.Y == 0;
        var vertically = direction.X == 0;
        var knight = Math.Abs((int)direction.X) == 2 && Math.Abs((int)direction.Y) == 1
                     || Math.Abs((int)direction.X) == 1 && Math.Abs((int)direction.Y) == 2;

        if (!(diagonally || horizontally || vertically))
        {
            // Knights are always unobstructed
            return knight;
        }

        if (vertically)
        {
            Debug.Assert(first.File == second.File);

            var start = Math.Min(first.Rank, second.Rank) + 1;
            var end = Math.Max(first.Rank, second.Rank) - 1;

            for (var i = start; i <= end; ++i)
            {
                if (pieces.ContainsKey(new Square(first.File, i)))
                {
                    return false;
                }
            }

            return true;
        }
        else if (horizontally)
        {
            Debug.Assert(first.Rank == second.Rank);

            var start = Math.Min(first.File, second.File) + 1;
            var end = Math.Max(first.File, second.File) - 1;

            for (var i = start; i <= end; ++i)
            {
                if (pieces.ContainsKey(new Square(i, first.Rank)))
                {
                    return false;
                }
            }

            return true;
        }
        else if (diagonally)
        {
            // We know that the direction is not horizontal or vertical, so it must be diagonally

            var stepX = direction.X > 0 ? -1 : 1;
            var stepY = direction.Y > 0 ? -1 : 1;

            for (var i = 1; i < Math.Abs(direction.X); ++i)
            {
                var square = new Square(first.File + i * stepX, first.Rank + i * stepY);

                if (pieces.TryGetValue(square, out var piece))
                {
                    _logger?.LogTrace("{} is blocking everything!", piece);
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    public IEnumerable<ChessPiece> GetAttackers(Square square, Color color)
    {
        return GetAttackers(square, color, Pieces.Items.ToDictionary(x => x.Square));
    }

    private IEnumerable<ChessPiece> GetAttackers(Square square, Color color, IDictionary<Square, ChessPiece> pieces)
    {
        return pieces.Values.Where(p => p.Color == color && CanMoveTo(p, square, pieces));
    }

    public bool IsGameFinished()
    {
        return false;
    }

    private bool IsLegalMove(Move? move)
    {
        //TODO: This still needs to account for en passant captures and castling.

        if (IsGameFinished())
        {
            _logger?.LogDebug("Move is not legal because the game has already ended.");
            return false;
        }

        if (move == null)
        {
            _logger?.LogDebug("Move is not legal because it was null.");
            return false;
        }

        if (!move.From.IsValid() || !move.To.IsValid())
        {
            _logger?.LogDebug("Move is not legal because either from or two are not valid squares.");
            return false;
        }

        if (move.From == move.To)
        {
            _logger?.LogDebug("Move is illegal because it is an empty move.");
            return false;
        }

        var maybePiece = Pieces.Lookup(move.From);
        if (!maybePiece.HasValue)
        {
            _logger?.LogDebug("Move is not legal because there is no piece on the from square.");
            return false;
        }

        var piece = maybePiece.Value;
        if (piece.Color != CurrentTurn)
        {
            _logger?.LogDebug("Move is not legal because the piece on the from square has the wrong color.");
            return false;
        }

        if (!CanMoveTo(piece, move.To))
        {
            _logger?.LogDebug("Move is not legal because the piece can't move there.");
            return false;
        }

        if (!UnobstructedPathExists(move.From, move.To, Pieces.Items.ToDictionary(x => x.Square)))
        {
            _logger?.LogDebug("Move is not legal because the path is obstructed.");
            return false;
        }


        var king = Pieces.Items.First(p => p.Color == CurrentTurn && p.Type == PieceType.King);
        var kingSquare = king.Square == move.From ? move.To : king.Square;

        // Figure out if moving this piece would put the king in check or if the king currently is in check
        // First, we make the move and get all pieces. Next, we check if there are any attackers on the king.
        var boardPieces = Pieces.Items.ToDictionary(p => p.Square);
        boardPieces.Remove(move.From);
        boardPieces[move.To] = piece with { Square = move.To };

        var checkingPieces = GetAttackers(kingSquare, CurrentTurn.Flipped(), boardPieces).ToList();
        if (checkingPieces.Any())
        {
            var str = checkingPieces.Aggregate("", (acc, p) => acc + p.ToString() + ", ");
            _logger?.LogDebug("Move is not legal because [{str}] would be putting the {CurrentTurn} King in check.", str, CurrentTurn);
            return false;
        }

        return true;
    }

    public bool MakeMove(Move? move)
    {
        if (!IsLegalMove(move))
            return false;

        if (move != null)
            MakeLegalNonNullMove(move);

        HalfMoveClock = (HalfMoveClock + 1) % 2;
        if (CurrentTurn == Color.Black)
        {
            FullMoveNumber++;
        }

        CurrentTurn = CurrentTurn.Flipped();
        _moves.Push(move);

        return true;
    }

    private void MakeLegalNonNullMove(Move move)
    {
        var piece = Pieces.Lookup(move.From);
        if (!piece.HasValue)
            throw new InvalidOperationException("A legal move turns out to be illegal!");

        Pieces.Edit(a =>
        {
            a.Remove(move.From);
            a.AddOrUpdate(piece.Value with { Square = move.To });
        });

        Debug.WriteLine($"Moved {piece.Value.Color} {piece.Value.Type} from {(char)('A' + move.From.File)}{move.From.Rank + 1} to {(char)('A' + move.To.File)}{move.To.Rank + 1}.");

        _moves.Push(move);
    }

    public void UndoMove()
    {
        _moves.Pop();
    }
}

public class IllegalFenException : Exception
{
    public IllegalFenException(string message) : base(message)
    {
    }

    public IllegalFenException(string message, Exception innerException) : base(message, innerException) { }
}