namespace ChessRepertoire.Model.Piece;

public interface IPieceVisitor<out T> {
    T Visit(Pawn pawn);
    T Visit(Knight knight);
    T Visit(Bishop bishop);
    T Visit(Rook rook);
    T Visit(Queen queen);
    T Visit(King king);
}

public interface IPieceVisitor
{
    void Visit(Pawn pawn);
    void Visit(Knight knight);
    void Visit(Bishop bishop);
    void Visit(Rook rook);
    void Visit(Queen queen);
    void Visit(King king);
}