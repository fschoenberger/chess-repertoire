using ChessRepertoire.Model.Piece;

namespace ChessRepertoire.ViewModel.Board;

public interface IPieceViewModel {
    Piece Piece { get; }
    int Row { get; }
    int Column { get; }
}