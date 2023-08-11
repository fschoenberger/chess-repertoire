namespace ChessRepertoire.Model.Board;

internal static class PieceExtensions
{
    public static char ToFenSymbol(this ChessPiece piece)
    {
        var ret = piece.Type switch
        {
            PieceType.King => "k",
            PieceType.Pawn => "p",
            PieceType.Knight => "n",
            PieceType.Bishop => "b",
            PieceType.Rook => "r",
            PieceType.Queen => "q",
            _ => throw new ArgumentOutOfRangeException(nameof(piece.Type), "Enum value is out of range!"), 
        };

        return (piece.Color == Color.Black ? ret : ret.ToUpper())[0];
    }
}
