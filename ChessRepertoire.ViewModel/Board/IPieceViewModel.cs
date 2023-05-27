using ChessRepertoire.Model.Board;

namespace ChessRepertoire.ViewModel.Board;

public interface IPieceViewModel {
    public PieceType Type { get; }
    public Color Color { get; }
    public int Rank { get; }
    public int File { get; }
}