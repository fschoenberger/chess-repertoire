using System.Diagnostics;
using System.Windows.Input;
using DynamicData;
using DynamicData.Cache;
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

    public Square? EnPassantTargetSquare => _enPassantTargetSquare;

    public CastlingRights CastlingRights => _castlingRights;

    public ICommand MoveCommand { get; }

    public ChessBoard(Color currentTurn, int halfMoveClock, int fullMoveNumber, IEnumerable<ChessPiece> pieces, CastlingRights castlingRights, Square? enPassantTargetSquare = null)
    {
        MoveCommand = ReactiveCommand.Create<Move, bool>(MakeMove);

        CurrentTurn = currentTurn;
        HalfMoveClock = halfMoveClock;
        FullMoveNumber = fullMoveNumber;
        _castlingRights = castlingRights;
        _enPassantTargetSquare = enPassantTargetSquare;

        Pieces.Edit(c => {
            foreach (var piece in pieces) {
                c.AddOrUpdate(piece);
            }
        });
    }

    private bool IsLegalMove(Move? move)
    {
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

        CurrentTurn = CurrentTurn == Color.White ? Color.Black : Color.White;

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