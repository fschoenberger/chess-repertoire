using System.Numerics;

namespace ChessRepertoire.Model.Board;

public enum PieceType
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}


public record ChessPiece(Color Color, PieceType Type, Square Square)
{
    /// <summary>
    /// Returns if a piece may move to this square. This method returns true for all conceivable methods of movement for a piece.
    /// It is the caller's job to determine if this particular movement is actually allowed (e.g., pawns may only capture diagonally). 
    /// </summary>
    /// <param name="target">The target square to move to.</param>
    /// <returns>True if the piece can theoretically move there, false otherwise.</returns>
    public bool CanMoveTo(Square target)
    {
        var pieceVector = new Vector2(Square.File, Square.Rank);
        var targetVector = new Vector2(target.File, target.Rank);

        var direction = targetVector - pieceVector;

        // We don't allow null moves
        if (direction is { X: 0, Y: 0 } || !target.IsValid())
            return false;

        switch (Type)
        {
            case PieceType.Pawn:
                var forwardDirection = Color == Color.White ? 1 : -1;
                var startingRank = Color == Color.White ? 1 : 6; // Caution: Zero-based indexing

                // Pawns can move forward by one square or two squares if it's their first move
                if (direction.X == 0 && (int)direction.Y == forwardDirection)
                    return true;
                if (Square.Rank == startingRank && direction.X == 0 && (int)direction.Y == 2 * forwardDirection)
                    return true;

                // Pawns can capture diagonally
                return Math.Abs((int)direction.X) == 1 && (int)direction.Y == forwardDirection;

            case PieceType.Knight:
                // Knights can capture by moving in an L-shape pattern:
                return Math.Abs((int)direction.X) == 1 && Math.Abs((int)direction.Y) == 2 ||
                       Math.Abs((int)direction.X) == 2 && Math.Abs((int)direction.Y) == 1;

            case PieceType.Bishop:
                return Math.Abs((int)direction.X) == Math.Abs((int)direction.Y);

            case PieceType.Rook:
                return direction.X == 0 || direction.Y == 0;

            case PieceType.Queen:
                return Math.Abs((int)direction.X) == Math.Abs((int)direction.Y) ||
                       direction.X == 0 || direction.Y == 0;

            case PieceType.King:
                return Math.Abs((int)direction.X) <= 1 && Math.Abs((int)direction.Y) <= 1;
        }

        return false;
    }

    public override string ToString()
    {
        return $"{Color} {Type} at {Square}";
    }
}
