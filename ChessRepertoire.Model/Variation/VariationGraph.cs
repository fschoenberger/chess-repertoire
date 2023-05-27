using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessRepertoire.Model.Variation {
    public class VariationGraph {
        private readonly Dictionary<Position, List<Position>> _adjacencyList;
        private readonly HashSet<Position> _nodes;

        public static Position InitialPosition { get; } = new Position { FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR" };

        public VariationGraph() {
            _adjacencyList = new Dictionary<Position, List<Position>>();
            _nodes = new HashSet<Position>();

            AddNode(InitialPosition);
        }

        public void AddNode(Position node) {
            if (_nodes.Contains(node)) return;

            _nodes.Add(node);
            _adjacencyList.Add(node, new List<Position>());
        }

        public void AddEdge(Position from, Position to) {
            if (!_nodes.Contains(from)) throw new ArgumentException("Node not found", nameof(from));
            if (!_nodes.Contains(to)) throw new ArgumentException("Node not found", nameof(to));
            _adjacencyList[from].Add(to);
        }

        public IEnumerable<Position> GetChildren(Position position) {
            if (!_nodes.Contains(position)) throw new ArgumentException("Node not found", nameof(position));
            return _adjacencyList[position];
        }

        public IEnumerable<Position> GetPositions() {
            return _nodes;
        }
    }

    public class Position {
        public string FEN { get; set; }
    }

    internal class Edge {
        Position To { get; set; }
    }
}
