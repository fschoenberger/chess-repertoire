using System.Diagnostics;
using System.Windows.Input;
using DynamicData;
using DynamicData.Cache;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ChessRepertoire.Model.Board;

[Flags]
public enum CastlingRights {
    None = 0,
    WhiteKingSide = 1,
    WhiteQueenSide = 2,
    BlackKingSide = 4,
    BlackQueenSide = 8
}

public class ChessBoard : ReactiveObject {
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

    public Square? EnPassantTargetSquare => _enPassantTargetSquare;

    public CastlingRights CastlingRights => _castlingRights;

    public ICommand MoveCommand { get; }

    public ChessBoard(string position = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1") {
        MoveCommand = ReactiveCommand.Create<Move, bool>(MakeMove);

        var components = position.Split(' ');

        var piecePlacement = components[0];
        if (piecePlacement.Count(c => c == 'k') != 1)
            throw new IllegalFenException("There must be exactly one black King!");

        if (piecePlacement.Count(c => c == 'K') != 1)
            throw new IllegalFenException("There must be exactly one white King!");

        // FEN is from rank 8 to rank 1, but we store it from rank 1 to rank 8
        var ranks = piecePlacement.Split('/').Reverse().ToArray();

        if (ranks.Length != 8)
            throw new IllegalFenException($"The format must describe 8 ranks, got {ranks.Length} instead.");

        Pieces.Edit(innerCache => {
            for (var rank = 0; rank < 8; rank++) {
                var rankString = ranks[rank];

                for (var file = 0; file < 8; file++) {
                    var piece = rankString[file];

                    if (piece is >= '1' and <= '8') {
                        file += piece - '0' - 1;
                        continue;
                    }

                    var color = char.IsUpper(piece) ? Color.White : Color.Black;
                    var pieceType = char.ToLower(piece) switch {
                        'p' => PieceType.Pawn,
                        'n' => PieceType.Knight,
                        'b' => PieceType.Bishop,
                        'r' => PieceType.Rook,
                        'q' => PieceType.Queen,
                        'k' => PieceType.King,
                        _ => throw new IllegalFenException($"Piece type must be one of p, n, b, r, q, k, instead is {piece}")
                    };
                    innerCache.AddOrUpdate(new ChessPiece(color, pieceType, new Square(file, rank)));
                }
            }
        });

        CurrentTurn = components[1] == "w" ? Color.White : (components[1] == "b" ? Color.Black : throw new IllegalFenException($"Piece color must be either black or white, instead is {components[1]}"));

        var castlingAvailability = components[2];
        if (castlingAvailability.Any(c => c != '-' && c != 'K' && c != 'k' && c != 'Q' && c != 'q'))
            throw new IllegalFenException($"Castling availability must be one of -, K, k, Q, q, instead is {castlingAvailability}");

        _castlingRights = CastlingRights.None;
        _castlingRights |= castlingAvailability.Contains('K') ? CastlingRights.WhiteKingSide : CastlingRights.None;
        _castlingRights |= castlingAvailability.Contains('Q') ? CastlingRights.WhiteQueenSide : CastlingRights.None;
        _castlingRights |= castlingAvailability.Contains('k') ? CastlingRights.BlackKingSide : CastlingRights.None;
        _castlingRights |= castlingAvailability.Contains('q') ? CastlingRights.BlackQueenSide : CastlingRights.None;

        var enPassantTargetSquare = components[3];
        if (enPassantTargetSquare != "-") {
            var file = enPassantTargetSquare[0] - 'a';
            var rank = enPassantTargetSquare[1] - '1';
            _enPassantTargetSquare = new Square(file, rank);
        }

        try {
            HalfMoveClock = Convert.ToInt32(components[4]);
        } catch (FormatException e) {
            throw new IllegalFenException($"Half move clock must be an integer, instead is {components[4]}", e);
        }

        try {
            FullMoveNumber = Convert.ToInt32(components[5]);
        } catch (FormatException e) {
            throw new IllegalFenException($"Full move number must be an integer, instead is {components[5]}", e);
        }
    }

    private bool IsLegalMove(Move? move) {
        return true;
    }

    public bool MakeMove(Move? move) {
        if (!IsLegalMove(move))
            return false;

        if (move != null)
            MakeLegalNonNullMove(move);

        HalfMoveClock = (HalfMoveClock + 1) % 2;
        if (CurrentTurn == Color.Black) {
            FullMoveNumber++;
        }

        CurrentTurn = CurrentTurn == Color.White ? Color.Black : Color.White;

        _moves.Push(move);

        return true;
    }

    private void MakeLegalNonNullMove(Move move) {
        var piece = Pieces.Lookup(move.From);
        if (!piece.HasValue)
            throw new InvalidOperationException("A legal move turns out to be illegal!");

        Pieces.Edit(a => {
            a.Remove(move.From);
            a.AddOrUpdate(piece.Value with { Square = move.To });
        });

        Debug.WriteLine($"Moved {piece.Value.Color} {piece.Value.Type} from {(char)('A' + move.From.File)}{move.From.Rank + 1} to {(char)('A' + move.To.File)}{move.To.Rank + 1}.");

        _moves.Push(move);
    }

    public void UndoMove() {
        _moves.Pop();
    }
}

public class IllegalFenException : Exception {
    public IllegalFenException(string message) : base(message) {
    }

    public IllegalFenException(string message, Exception innerException) : base(message, innerException) { }
}