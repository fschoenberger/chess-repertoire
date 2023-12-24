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

    [Reactive]
    private GameState CurrentGameState { get; set; }

    private readonly ObservableAsPropertyHelper<Color> _currentTurn;
    public Color CurrentTurn => _currentTurn.Value;

    private readonly ObservableAsPropertyHelper<CastlingRights> _castlingRights;
    public CastlingRights CastlingRights => _castlingRights.Value;

    private readonly ObservableAsPropertyHelper<Square?> _enPassantTargetSquare;
    public Square? EnPassantTargetSquare => _enPassantTargetSquare.Value;

    private readonly ObservableAsPropertyHelper<IReadOnlyDictionary<Square, ChessPiece>> _pieces;
    public IReadOnlyDictionary<Square, ChessPiece> Pieces => _pieces.Value;

    private ISourceCache<GameState, Guid> _gameStates = new SourceCache<GameState, Guid>(x => x.Id);
    public IObservableCache<GameState, Guid> GameStates => _gameStates;

    public ChessGame(
        IEnumerable<ChessPiece> pieces,
        Color currentTurn = Color.White,
        int fullMoveNumber = 1,
        CastlingRights castlingRights = CastlingRights.BlackKingSide | CastlingRights.BlackQueenSide | CastlingRights.WhiteKingSide | CastlingRights.WhiteQueenSide,
        Square? enPassantTargetSquare = null,
        ILogger<ChessGame>? logger = null) {

        var state = new GameState(
            new Position(
            pieces.ToDictionary(x => x.Square),
            enPassantTargetSquare,
            castlingRights,
            currentTurn),
            fullMoveNumber,
            null
        );

        _gameStates.AddOrUpdate(state);
        CurrentGameState = state;

        _logger = logger ?? new DebugLoggerProvider().CreateLogger(nameof(ChessGame));

        var currentBoardState = this.WhenAnyValue(x => x.CurrentGameState);

        _currentTurn = currentBoardState.Select(x => x.CurrentTurn).ToProperty(this, x => x.CurrentTurn);
        _castlingRights = currentBoardState.Select(x => x.CastlingRights).ToProperty(this, x => x.CastlingRights);
        _enPassantTargetSquare = currentBoardState.Select(x => x.EnPassantTargetSquare).ToProperty(this, x => x.EnPassantTargetSquare);
        _pieces = currentBoardState.Select(x => x.Pieces).ToProperty(this, x => x.Pieces);
    }

    public void MakeMove(Move move) {
        CurrentGameState = CurrentGameState.WithMove(move);
    }
}