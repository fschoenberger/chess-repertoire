namespace ChessRepertoire.Model.Board;

public enum PieceType {
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}


public record ChessPiece(Color Color, PieceType Type, Square Square);
