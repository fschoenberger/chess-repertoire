using ChessRepertoire.Model.Board;

namespace ChessRepertoire.ViewModel.Board;

internal class PieceViewModel : IPieceViewModel {
    public PieceViewModel(ChessPiece chessPiece) {
        Type = chessPiece.Type;
        Color = chessPiece.Color;
        Rank = chessPiece.Square.Rank;
        File = chessPiece.Square.File;
    }

    internal PieceViewModel(PieceType type, Color color, int rank, int file) {
        Type = type;
        Color = color;
        Rank = rank;
        File = file;
    }

    public PieceType Type { get; }
    public Color Color { get; }
    public int Rank { get;  }
    public int File { get; }
}