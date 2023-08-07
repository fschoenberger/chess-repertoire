using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessRepertoire.Model.Board;

namespace ChessRepertoire.Model.Tests.Board
{
    [TestClass]
    public class ChessPieceTest
    {
        [TestMethod]
        public void KingCanMoveOneSquare()
        {
            var piece = new ChessPiece(Color.White, PieceType.King, new Square(2, 2));

            Assert.IsTrue(piece.CanMoveTo(new Square(1, 1)));
            Assert.IsTrue(piece.CanMoveTo(new Square(1, 2)));
            Assert.IsTrue(piece.CanMoveTo(new Square(1, 3)));
            Assert.IsTrue(piece.CanMoveTo(new Square(2, 1)));
            Assert.IsTrue(piece.CanMoveTo(new Square(2, 3)));
            Assert.IsTrue(piece.CanMoveTo(new Square(3, 1)));
            Assert.IsTrue(piece.CanMoveTo(new Square(3, 2)));
            Assert.IsTrue(piece.CanMoveTo(new Square(3, 3)));

            // And kings can only move one square
            Assert.IsFalse(piece.CanMoveTo(new Square(2, 4)));
        }

        [TestMethod]
        public void KingCantDoNullMoves()
        {
            var piece = new ChessPiece(Color.White, PieceType.King, new Square(2, 2));
            Assert.IsFalse(piece.CanMoveTo(new Square(2, 2)));
        }

        [TestMethod]
        public void KingRespectsBoundaries()
        {
            var piece = new ChessPiece(Color.White, PieceType.King, new Square(0, 0));

            Assert.IsFalse(piece.CanMoveTo(new Square(-1, -1)));
            Assert.IsFalse(piece.CanMoveTo(new Square(-1, 0)));
            Assert.IsFalse(piece.CanMoveTo(new Square(-1, 1)));
            Assert.IsFalse(piece.CanMoveTo(new Square(0, -1)));
            Assert.IsFalse(piece.CanMoveTo(new Square(1, -1)));

            Assert.IsTrue(piece.CanMoveTo(new Square(0, 1)));
            //Assert.IsTrue(piece.CanMoveTo(new Square(0, 0)));
            Assert.IsTrue(piece.CanMoveTo(new Square(1, 1)));
        }


        [TestMethod]
        public void RookCanMoveHorizontallyAndVertically()
        {
            var piece = new ChessPiece(Color.White, PieceType.Rook, new Square(4, 4));

            // Horizontal moves
            Assert.IsTrue(piece.CanMoveTo(new Square(4, 0)));
            Assert.IsTrue(piece.CanMoveTo(new Square(4, 1)));
            Assert.IsTrue(piece.CanMoveTo(new Square(4, 2)));
            Assert.IsTrue(piece.CanMoveTo(new Square(4, 3)));
            Assert.IsTrue(piece.CanMoveTo(new Square(4, 5)));
            Assert.IsTrue(piece.CanMoveTo(new Square(4, 6)));
            Assert.IsTrue(piece.CanMoveTo(new Square(4, 7)));


            // Vertical moves
            Assert.IsTrue(piece.CanMoveTo(new Square(0, 4)));
            Assert.IsTrue(piece.CanMoveTo(new Square(1, 4)));
            Assert.IsTrue(piece.CanMoveTo(new Square(2, 4)));
            Assert.IsTrue(piece.CanMoveTo(new Square(3, 4)));
            Assert.IsTrue(piece.CanMoveTo(new Square(5, 4)));
            Assert.IsTrue(piece.CanMoveTo(new Square(6, 4)));
            Assert.IsTrue(piece.CanMoveTo(new Square(7, 4)));

            // Invalid moves
            Assert.IsFalse(piece.CanMoveTo(new Square(5, 5)));
            Assert.IsFalse(piece.CanMoveTo(new Square(3, 3)));
        }

        [TestMethod]
        public void BishopCanMoveDiagonally()
        {
            var piece = new ChessPiece(Color.White, PieceType.Bishop, new Square(3, 3));

            Assert.IsTrue(piece.CanMoveTo(new Square(1, 1)));
            Assert.IsTrue(piece.CanMoveTo(new Square(5, 1)));
            Assert.IsTrue(piece.CanMoveTo(new Square(1, 5)));
            Assert.IsTrue(piece.CanMoveTo(new Square(5, 5)));

            Assert.IsFalse(piece.CanMoveTo(new Square(2, 5)));
            Assert.IsFalse(piece.CanMoveTo(new Square(5, 2)));
        }

        [TestMethod]
        public void QueenCanMoveHorizontallyVerticallyAndDiagonally()
        {
            var piece = new ChessPiece(Color.White, PieceType.Queen, new Square(4, 4));

            Assert.IsTrue(piece.CanMoveTo(new Square(1, 4)));
            Assert.IsTrue(piece.CanMoveTo(new Square(7, 4)));
            Assert.IsTrue(piece.CanMoveTo(new Square(4, 1)));
            Assert.IsTrue(piece.CanMoveTo(new Square(4, 7)));

            Assert.IsTrue(piece.CanMoveTo(new Square(2, 2)));
            Assert.IsTrue(piece.CanMoveTo(new Square(6, 6)));

            Assert.IsFalse(piece.CanMoveTo(new Square(5, 2)));
            Assert.IsFalse(piece.CanMoveTo(new Square(2, 5)));
        }

        [TestMethod]
        public void KnightCanMoveInLShape()
        {
            var piece = new ChessPiece(Color.White, PieceType.Knight, new Square(3, 3));

            Assert.IsTrue(piece.CanMoveTo(new Square(1, 2)));
            Assert.IsTrue(piece.CanMoveTo(new Square(1, 4)));
            Assert.IsTrue(piece.CanMoveTo(new Square(2, 1)));
            Assert.IsTrue(piece.CanMoveTo(new Square(2, 5)));
            Assert.IsTrue(piece.CanMoveTo(new Square(4, 1)));
            Assert.IsTrue(piece.CanMoveTo(new Square(4, 5)));
            Assert.IsTrue(piece.CanMoveTo(new Square(5, 2)));
            Assert.IsTrue(piece.CanMoveTo(new Square(5, 4)));

            Assert.IsFalse(piece.CanMoveTo(new Square(4, 3)));
            Assert.IsFalse(piece.CanMoveTo(new Square(2, 3)));
        }

        [TestMethod]
        public void PawnCanMoveForwardOneSquare()
        {
            var piece = new ChessPiece(Color.White, PieceType.Pawn, new Square(2, 2));

            Assert.IsTrue(piece.CanMoveTo(new Square(2, 3)));

            Assert.IsFalse(piece.CanMoveTo(new Square(2, 1)));
            Assert.IsFalse(piece.CanMoveTo(new Square(2, 4)));
        }

        [TestMethod]
        public void PawnCanMoveTwoSquaresOnFirstMove()
        {
            var piece = new ChessPiece(Color.White, PieceType.Pawn, new Square(2, 1));

            Assert.IsTrue(piece.CanMoveTo(new Square(2, 3)));

            Assert.IsFalse(piece.CanMoveTo(new Square(2, 4)));
            Assert.IsFalse(piece.CanMoveTo(new Square(2, 5)));
        }

        [TestMethod]
        public void PawnCanCaptureDiagonally()
        {
            var piece = new ChessPiece(Color.White, PieceType.Pawn, new Square(2, 2));

            Assert.IsTrue(piece.CanMoveTo(new Square(3, 3)));
            Assert.IsTrue(piece.CanMoveTo(new Square(1, 3)));

            Assert.IsFalse(piece.CanMoveTo(new Square(3, 2)));
            Assert.IsFalse(piece.CanMoveTo(new Square(1, 2)));
        }
    }
}
