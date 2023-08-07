using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessRepertoire.Infrastructure;
using ChessRepertoire.Model.Board;

namespace ChessRepertoire.Model.Tests.Board;


[TestClass]
public class ChessBoardTest
{
    [TestMethod]
    public void WhitePawnCanMoveTwoSpacesFromBaseWhenUnobstructed()
    {
        var pawn = new ChessPiece(Color.White, PieceType.Pawn, new Square(4, 1));

        var board = new ChessBoard(new List<ChessPiece> {
            new(Color.White, PieceType.King, new Square(4, 0)),
            new(Color.Black, PieceType.King, new Square(4, 7)),
            pawn
        });

        Assert.IsTrue(board.CanMoveTo(pawn, new Square(4, 2)));
        Assert.IsTrue(board.CanMoveTo(pawn, new Square(4, 3)));
        Assert.IsFalse(board.CanMoveTo(pawn, new Square(4, 4)));
    }

    [TestMethod]
    public void BlackPawnCanMoveTwoSpacesFromBaseWhenUnobstructed()
    {
        var pawn = new ChessPiece(Color.Black, PieceType.Pawn, new Square(4, 6));

        var board = new ChessBoard(new List<ChessPiece> {
            new(Color.White, PieceType.King, new Square(4, 0)),
            new(Color.Black, PieceType.King, new Square(4, 7)),
            pawn
        }, Color.Black);

        Assert.IsTrue(board.CanMoveTo(pawn, new Square(4, 5)));
        Assert.IsTrue(board.CanMoveTo(pawn, new Square(4, 4)));
        Assert.IsFalse(board.CanMoveTo(pawn, new Square(4, 3)));
    }

    [TestMethod]
    public void PawnCantMoveWhenObstructed()
    {
        var pawn = new ChessPiece(Color.White, PieceType.Pawn, new Square(4, 1));

        var board = new ChessBoard(new List<ChessPiece> {
            new(Color.White, PieceType.King, new Square(4, 0)),
            new(Color.Black, PieceType.King, new Square(4, 7)),
            new(Color.Black, PieceType.Pawn, new Square(4, 2)),
            pawn
        });

        Assert.IsFalse(board.CanMoveTo(pawn, new Square(4, 2)));
        Assert.IsFalse(board.CanMoveTo(pawn, new Square(4, 3)));
        Assert.IsFalse(board.CanMoveTo(pawn, new Square(4, 4)));
    }

    //[TestMethod]
    //public void BoardCtorInitializesCorrectly() {
    //    var board = new ChessBoard();
    //    Assert.AreEqual(Color.White, board.CurrentTurn);
    //    Assert.AreEqual(0, board.HalfMoveClock);
    //    Assert.AreEqual(1, board.FullMoveNumber);
    //    Assert.IsNull(board.EnPassantTargetSquare);
    //    Assert.AreEqual(CastlingRights.WhiteKingSide | CastlingRights.WhiteQueenSide | CastlingRights.BlackKingSide | CastlingRights.BlackQueenSide, board.CastlingRights);
    //}

    //[TestMethod]
    //public void BoardCtorInitializesCorrectlyWithFen() {
    //    var board = new ChessBoard(ChessBoard.InitialPositionFen);
    //    Assert.AreEqual(Color.White, board.CurrentTurn);
    //    Assert.AreEqual(0, board.HalfMoveClock);
    //    Assert.AreEqual(1, board.FullMoveNumber);
    //    Assert.IsNull(board.EnPassantTargetSquare);
    //    Assert.AreEqual(CastlingRights.WhiteKingSide | CastlingRights.WhiteQueenSide | CastlingRights.BlackKingSide | CastlingRights.BlackQueenSide, board.CastlingRights);
    //}

    //[TestMethod]
    //public void BoardCtorThrowsOnTooFewRanks() {
    //    Assert.ThrowsException<IllegalFenException>(() => new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP w KQkq - 0 1"));
    //}

    //[TestMethod]
    //public void BoardCtorThrowsOnTooManyRanks() {
    //    Assert.ThrowsException<IllegalFenException>(() => new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/8 w KQkq - 0 1"));
    //}

    //[TestMethod]
    //public void BoardCtorThrowsOnTooManyWhiteKings() {
    //    Assert.ThrowsException<IllegalFenException>(() => new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBKKBNR w KQkq - 0 1"));
    //}

    //[TestMethod]
    //public void BoardCtorThrowsOnTooManyBlackKings() {
    //    Assert.ThrowsException<IllegalFenException>(() => new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/rNBQKBNR w KQkq - 0 1"));
    //}

    //[TestMethod]
    //public void BoardCtorThrowsOnTooFewWhiteKings() {
    //    Assert.ThrowsException<IllegalFenException>(() => new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBN w KQkq - 0 1"));
    //}

    //[TestMethod]
    //public void BoardCtorThrowsOnTooFewBlackKings() {
    //    Assert.ThrowsException<IllegalFenException>(() => new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/rNBQKBN w KQkq - 0 1"));
    //}
}
