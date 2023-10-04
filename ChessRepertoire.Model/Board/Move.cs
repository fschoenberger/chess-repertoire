namespace ChessRepertoire.Model.Board;

public record Move(Square From, Square To, ChessPiece? PromotedPiece = null) {
    public bool IsPotentiallyValid() {
        return From.IsValid() && To.IsValid();
    }

    public override string ToString() {
        return $"{From}{To}";
    }
}