namespace ChessRepertoire.Model.Piece;

public interface IPieceVisitable {
    void Accept(IPieceVisitor visitor);

    T Accept<T>(IPieceVisitor<T> visitor);
}