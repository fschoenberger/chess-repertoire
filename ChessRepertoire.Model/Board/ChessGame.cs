using System.Reactive.Linq;
using DynamicData;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
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

public class ChessGame : ReactiveObject {
    private readonly ILogger? _logger;

    private readonly ObservableAsPropertyHelper<BoardState> _currentBoardState;
    private BoardState CurrentBoardState => _currentBoardState.Value;

    private readonly ObservableAsPropertyHelper<Color> _currentTurn;
    public Color CurrentTurn => _currentTurn.Value;

    private readonly ObservableAsPropertyHelper<CastlingRights> _castlingRights;
    public CastlingRights CastlingRights => _castlingRights.Value;

    //public readonly ObservableAsPropertyHelper<int> _halfMoveClock;
    //public int HalfMoveClock => _halfMoveClock.Value;

    //private readonly ObservableAsPropertyHelper<int> _fullMoveNumber;
    //public int FullMoveNumber => _fullMoveNumber.Value;

    private readonly ObservableAsPropertyHelper<Square?> _enPassantTargetSquare;
    public Square? EnPassantTargetSquare => _enPassantTargetSquare.Value;

    private readonly ObservableAsPropertyHelper<IReadOnlyDictionary<Square, ChessPiece>> _pieces;
    public IReadOnlyDictionary<Square, ChessPiece> Pieces => _pieces.Value;

    [Reactive]
    public ulong CurrentId { get; private set; }

    private readonly ISourceCache<BoardState, ulong> _boardStates = new SourceCache<BoardState, ulong>(x => x.Id);
    public IObservableCache<BoardState, ulong> BoardStates => _boardStates;

    public ChessGame(
        IEnumerable<ChessPiece> pieces,
        Color currentTurn = Color.White,
        int halfMoveClock = 0,
        int fullMoveNumber = 1,
        CastlingRights castlingRights = CastlingRights.BlackKingSide | CastlingRights.BlackQueenSide | CastlingRights.WhiteKingSide | CastlingRights.WhiteQueenSide,
        Square? enPassantTargetSquare = null,
        ILogger<ChessGame>? logger = null) {
        var state = new BoardState(
            pieces.ToDictionary(x => x.Square),
            enPassantTargetSquare,
            castlingRights,
            currentTurn
        );

        _boardStates.AddOrUpdate(state);
        CurrentId = state.Id;

        _logger = logger ?? new DebugLoggerProvider().CreateLogger(nameof(ChessGame));

        var currentBoardState = this
            .WhenAnyValue(x => x.CurrentId)
            .Select(id => _boardStates.Items.FirstOrDefault(s => s.Id == id))
            .WhereNotNull(); // I don't think this is needed, but it makes the compiler happy

        _currentBoardState = currentBoardState.ToProperty(this, x => x.CurrentBoardState);
        _currentTurn = currentBoardState.Select(x => x.CurrentTurn).ToProperty(this, x => x.CurrentTurn);
        _castlingRights = currentBoardState.Select(x => x.CastlingRights).ToProperty(this, x => x.CastlingRights);
        _enPassantTargetSquare = currentBoardState.Select(x => x.EnPassantTargetSquare).ToProperty(this, x => x.EnPassantTargetSquare);
        _pieces = currentBoardState.Select(x => x.Pieces).ToProperty(this, x => x.Pieces);
    }

    public bool MakeMove(Move? move) {
        try {
            var nextBoardState = CurrentBoardState.MakeMove(move);
            var id = nextBoardState.Id;

            if (!_boardStates.Keys.Contains(nextBoardState.Id)) {
                _boardStates.AddOrUpdate(nextBoardState);
            } else {
                nextBoardState = _boardStates.Items.FirstOrDefault(x => x.Id == nextBoardState.Id);
            }

            CurrentBoardState.AddChild(nextBoardState!);
            nextBoardState!.AddParent(CurrentBoardState);

            CurrentId = id;

            return true;
        }
        catch (InvalidOperationException) {
            return false;
        }
    }
}