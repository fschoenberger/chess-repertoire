namespace ChessRepertoire.Model.Board;

public enum Color {
    White,
    Black
}

public static class PieceColorExtension {
    public static Color Flipped(this Color c) {
        return c == Color.White ? Color.Black : Color.White;
    }
}