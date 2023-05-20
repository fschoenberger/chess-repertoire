using ChessRepertoire.Model.Piece;

namespace ChessRepertoire.ViewModel.Board;

internal class PieceViewModel : IPieceViewModel, IPieceVisitable {
    public Piece Piece { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    public void Accept(IPieceVisitor visitor)
    {
        Piece.Accept(visitor);
    }

    public T Accept<T>(IPieceVisitor<T> visitor)
    {
        return Piece.Accept<T>(visitor);
    }
}