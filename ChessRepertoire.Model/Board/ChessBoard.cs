using System.Globalization;
using System.Reactive.Linq;
using System.Security.Cryptography;
using DynamicData;
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
    private readonly ILogger? _logger;

    private readonly IDictionary<ulong, BoardState> _boardStates = new Dictionary<ulong, BoardState>();

    // Implementation note: We currently save our edges in two data structures. The reason is that we want to get a move's successor(s) 
    // as often as their predecessor(s) and it's cheaper to cache than to recalculate every time. 
    private readonly IDictionary<ulong, ISet<ulong>> _predecessors = new Dictionary<ulong, ISet<ulong>>();
    private readonly IDictionary<ulong, ISet<ulong>> _successors = new Dictionary<ulong, ISet<ulong>>();

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

    public ISourceList<BoardState> BoardStates { get; }

    public ChessBoard(
        IEnumerable<ChessPiece> pieces,
        Color currentTurn = Color.White,
        int halfMoveClock = 0,
        int fullMoveNumber = 1,
        CastlingRights castlingRights = CastlingRights.BlackKingSide | CastlingRights.BlackQueenSide | CastlingRights.WhiteKingSide | CastlingRights.WhiteQueenSide,
        Square? enPassantTargetSquare = null,
        ILogger<ChessBoard>? logger = null)
    {

        var state = new BoardState(
            pieces.ToDictionary(x => x.Square),
            enPassantTargetSquare,
            castlingRights,
            currentTurn
        );

        _boardStates[state.Id] = state;
        CurrentId = state.Id;

        _logger = logger ?? new DebugLoggerProvider().CreateLogger(nameof(ChessBoard));

        var currentBoardState = this.WhenAnyValue(x => x.CurrentId)
            .Select(id => _boardStates[id]);

        _currentBoardState = currentBoardState.ToProperty(this, x => x.CurrentBoardState);
        _currentTurn = currentBoardState.Select(x => x.CurrentTurn).ToProperty(this, x => x.CurrentTurn);
        _castlingRights = currentBoardState.Select(x => x.CastlingRights).ToProperty(this, x => x.CastlingRights);
        _enPassantTargetSquare = currentBoardState.Select(x => x.EnPassantTargetSquare).ToProperty(this, x => x.EnPassantTargetSquare);
        _pieces = currentBoardState.Select(x => x.Pieces).ToProperty(this, x => x.Pieces);
    }

    public bool MakeMove(Move? move)
    {
        try
        {

            var oldId = CurrentBoardState.Id;
            var nextBoardState = CurrentBoardState.MakeMove(move);
            var id = nextBoardState.Id;

            if (_successors.TryGetValue(oldId, out var successors))
            {
                successors!.Add(id);
            }
            else
            {
                _successors[oldId] = new HashSet<ulong> { id };
            }

            if (_predecessors.TryGetValue(id, out var predecessors))
            {
                predecessors!.Add(id);
            }
            else
            {
                _predecessors[id] = new HashSet<ulong> { oldId };
            }

            _boardStates[id] = nextBoardState;
            CurrentId = id;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}