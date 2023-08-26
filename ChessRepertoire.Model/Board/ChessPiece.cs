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

    private IEnumerable<Move> GetPotentialKnightMoves()
    {
        int[] knightMovesX = { 2, 1, -1, -2, -2, -1, 1, 2 };
        int[] knightMovesY = { 1, 2, 2, 1, -1, -2, -2, -1 };

        for (var i = 0; i < knightMovesX.Length; i++)
        {
            var move = new Move(Square, new Square(Square.File + knightMovesX[i], Square.Rank + knightMovesY[i]));
            if (move.IsPotentiallyValid())
                yield return move;
        }
    }

    private IEnumerable<Move> GetPotentialPawnMoves()
    {
        var forwardDirection = Color == Color.White ? 1 : -1;
        var startingRank = Color == Color.White ? 1 : 6; // Zero-based indexing

        // One step forward
        var forwardMove = new Move(Square, Square with { Rank = Square.Rank + forwardDirection });
        if (forwardMove.IsPotentiallyValid())
            yield return forwardMove;

        // Two steps forward if on starting rank
        if (Square.Rank == startingRank)
        {
            var doubleForwardMove = new Move(Square, Square with { Rank = Square.Rank + 2 * forwardDirection });
            if (doubleForwardMove.IsPotentiallyValid())
                yield return doubleForwardMove;
        }

        // Diagonal captures
        var diagonalRight = new Move(Square, new Square(Square.File + 1, Square.Rank + forwardDirection));
        if (diagonalRight.IsPotentiallyValid())
            yield return diagonalRight;

        var diagonalLeft = new Move(Square, new Square(Square.File - 1, Square.Rank + forwardDirection));
        if (diagonalLeft.IsPotentiallyValid())
            yield return diagonalLeft;
    }

    private IEnumerable<Move> GetPotentialRookMoves()
    {
        for (var i = 0; i < 8; ++i)
        {
            if (i != Square.File) yield return new Move(Square, Square with { File = i });
            if (i != Square.Rank) yield return new Move(Square, Square with { Rank = i });
        }
    }

    private IEnumerable<Move> GetPotentialBishopMoves()
    {
        // Diagonal moves
        for (var i = 1; i <= 7; i++)
        {
            var upRight = new Move(Square,new Square(Square.File + i, Square.Rank + i));
            if (upRight.IsPotentiallyValid())
                yield return upRight; // Diagonal up-right

            var downLeft = new Move(Square,new Square(Square.File - i, Square.Rank - i));
            if (downLeft.IsPotentiallyValid())
                yield return downLeft; // Diagonal down-left

            var downRight = new Move(Square,new Square(Square.File + i, Square.Rank - i));
            if (downRight.IsPotentiallyValid())
                yield return downRight; // Diagonal down-right

            var upLeft = new Move(Square,new Square(Square.File - i, Square.Rank + i));
            if (upLeft.IsPotentiallyValid())
                yield return upLeft; // Diagonal up-left
        }
    }

    private IEnumerable<Move> GetPotentialKingMoves()
    {
        int[] kingMovesX = { 0, 1, 1, 1, 0, -1, -1, -1 };
        int[] kingMovesY = { 1, 1, 0, -1, -1, -1, 0, 1 };

        for (var i = 0; i < kingMovesX.Length; i++)
        {
            var move = new Move(Square,new Square(Square.File + kingMovesX[i], Square.Rank + kingMovesY[i]));
            if (move.IsPotentiallyValid())
                yield return move;
        }
    }

    public IEnumerable<Move> GetPotentiallyLegalMoves()
    {
        return Type switch
        {
            PieceType.Pawn => GetPotentialPawnMoves(),
            PieceType.Knight => GetPotentialKnightMoves(),
            PieceType.Bishop => GetPotentialBishopMoves(),
            PieceType.Rook => GetPotentialRookMoves(),
            PieceType.King => GetPotentialKingMoves(),
            PieceType.Queen => GetPotentialRookMoves().Concat(GetPotentialBishopMoves()),
            _ => Enumerable.Empty<Move>(),
        };
    }

    public override string ToString()
    {
        return $"{Color} {Type} at {Square}";
    }
}
