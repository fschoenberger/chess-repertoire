namespace ChessRepertoire.Model.Board;

public class Edge {
    public UInt128 Id => ((UInt128)From << 64) | To;

    public string Move { get; init; }

    public ulong From { get; init; }
    public ulong To { get; init; }
}
