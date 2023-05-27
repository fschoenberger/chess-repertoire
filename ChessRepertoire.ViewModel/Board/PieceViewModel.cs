using ChessRepertoire.Model.Piece;

namespace ChessRepertoire.ViewModel.Board;

internal class PieceViewModel : IPieceViewModel, IPieceVisitable {
    public ChessPiece ChessPiece { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    public void Accept(IPieceVisitor visitor)
    {
        ChessPiece.Accept(visitor);
    }

    public T Accept<T>(IPieceVisitor<T> visitor)
    {
        return ChessPiece.Accept<T>(visitor);
    }
}