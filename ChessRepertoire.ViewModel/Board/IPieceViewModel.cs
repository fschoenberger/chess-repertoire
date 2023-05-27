using ChessRepertoire.Model.Piece;

namespace ChessRepertoire.ViewModel.Board;

public interface IPieceViewModel {
    ChessPiece ChessPiece { get; }
    int Row { get; }
    int Column { get; }
}