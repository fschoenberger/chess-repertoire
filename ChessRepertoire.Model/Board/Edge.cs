namespace ChessRepertoire.Model.Board;

public class Edge {
    public Move Move { get; init; }

    public GameState From { get; init; }
    public GameState To { get; init; }

    public Edge(Move move, GameState from, GameState to) {
        Move = move;
        From = from;
        To = to;
    }
}
