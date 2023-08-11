﻿using System.Reactive.Linq;
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

    public static string InitialPositionFen => "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private readonly ILogger? _logger;

    private readonly IDictionary<string, BoardState> _boardStates = new Dictionary<string, BoardState>();

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
    public string CurrentFen { get; private set; }

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
        CurrentFen = state.Id;

        _logger = logger ?? new DebugLoggerProvider().CreateLogger(nameof(ChessBoard));

        var currentBoardState = this.WhenAnyValue(x => x.CurrentFen)
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
            var nextBoardState = CurrentBoardState.MakeMove(move);

            _boardStates[nextBoardState.Id] = nextBoardState;
            CurrentFen = nextBoardState.Id;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}