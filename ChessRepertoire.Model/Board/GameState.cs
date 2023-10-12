using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace ChessRepertoire.Model.Board {
    public class GameState {
        private readonly Position _currentPosition;

        public CastlingRights CastlingRights => _currentPosition.CastlingRights;
        public Color CurrentTurn => _currentPosition.CurrentTurn;
        public Square? EnPassantTargetSquare => _currentPosition.EnPassantTargetSquare;
        public Dictionary<Square, ChessPiece> Pieces => _currentPosition.Pieces;

        public int FullMoveNumber { get; }

        private readonly ISourceCache<Edge, Move> _children = new SourceCache<Edge, Move>(x => x.Move);
        public IObservableCache<Edge, Move> Children => _children;

        public readonly GameState? Parent;

        public GameState(Position currentPosition, int fullMoveNumber, GameState? parent = null) {
            _currentPosition = currentPosition;
            FullMoveNumber = fullMoveNumber;
            Parent = parent;
        }

        public GameState WithMove(Move move) {
            var maybeRet = _children.Lookup(move);

            if (maybeRet.HasValue) {
                return maybeRet.Value.To;
            }

            var newFullMoveNumber = _currentPosition.CurrentTurn == Color.White ? FullMoveNumber : FullMoveNumber + 1;
            var newPosition = _currentPosition.WithMove(move);

            var ret = new GameState(newPosition, newFullMoveNumber, this);
            _children.AddOrUpdate(new Edge(move, this, ret));

            return ret;
        }
    }

}
