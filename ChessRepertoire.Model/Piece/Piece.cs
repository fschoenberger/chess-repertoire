namespace ChessRepertoire.Model.Piece;

public abstract class Piece : IPieceVisitable {
    public Color Color { get; set; }

    public abstract void Accept(IPieceVisitor visitor);
    public abstract T Accept<T>(IPieceVisitor<T> visitor);
}
