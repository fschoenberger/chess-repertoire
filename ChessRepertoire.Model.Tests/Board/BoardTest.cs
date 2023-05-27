using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessRepertoire.Model.Board;
using ChessRepertoire.Model.Piece;

namespace ChessRepertoire.Model.Tests.Board;


[TestClass]
public class BoardTest {
    [TestMethod]
    public void BoardCtorInitializesCorrectly() {
        var board = new ChessBoard();
        Assert.AreEqual(Color.White, board.CurrentTurn);
        Assert.AreEqual(0, board.HalfMoveClock);
        Assert.AreEqual(1, board.FullMoveNumber);
        Assert.IsNull(board.EnPassantTargetSquare);
        Assert.AreEqual(CastlingRights.WhiteKingSide | CastlingRights.WhiteQueenSide | CastlingRights.BlackKingSide | CastlingRights.BlackQueenSide, board.CastlingRights);
    }

    [TestMethod]
    public void BoardCtorInitializesCorrectlyWithFen() {
        var board = new ChessBoard(ChessBoard.InitialPositionFen);
        Assert.AreEqual(Color.White, board.CurrentTurn);
        Assert.AreEqual(0, board.HalfMoveClock);
        Assert.AreEqual(1, board.FullMoveNumber);
        Assert.IsNull(board.EnPassantTargetSquare);
        Assert.AreEqual(CastlingRights.WhiteKingSide | CastlingRights.WhiteQueenSide | CastlingRights.BlackKingSide | CastlingRights.BlackQueenSide, board.CastlingRights);
    }

    [TestMethod]
    public void BoardCtorThrowsOnTooFewRanks() {
        Assert.ThrowsException<IllegalFenException>(() => new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP w KQkq - 0 1"));
    }

    [TestMethod]
    public void BoardCtorThrowsOnTooManyRanks() {
        Assert.ThrowsException<IllegalFenException>(() => new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/8 w KQkq - 0 1"));
    }

    [TestMethod]
    public void BoardCtorThrowsOnTooManyWhiteKings() {
        Assert.ThrowsException<IllegalFenException>(() => new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBKKBNR w KQkq - 0 1"));
    }

    [TestMethod]
    public void BoardCtorThrowsOnTooManyBlackKings() {
        Assert.ThrowsException<IllegalFenException>(() => new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/rNBQKBNR w KQkq - 0 1"));
    }

    [TestMethod]
    public void BoardCtorThrowsOnTooFewWhiteKings() {
        Assert.ThrowsException<IllegalFenException>(() => new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBN w KQkq - 0 1"));
    }

    [TestMethod]
    public void BoardCtorThrowsOnTooFewBlackKings() {
        Assert.ThrowsException<IllegalFenException>(() => new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/rNBQKBN w KQkq - 0 1"));
    }
}
