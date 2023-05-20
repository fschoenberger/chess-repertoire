namespace ChessRepertoire.Model.Piece;

public class King : Piece {
    public King(Color color) {
        Color = color;
    }

    public override void Accept(IPieceVisitor visitor) {
        visitor.Visit(this);
    }

    public override T Accept<T>(IPieceVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}