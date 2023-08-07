using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessRepertoire.Model.Board;
using ChessRepertoire.Model.Variation;

namespace ChessRepertoire.Infrastructure
{
    public class FenRepository : IChessBoardRepository
    {
        public ChessBoard FromFen(string position)
        {
            var components = position.Split(' ');

            var piecePlacement = components[0];
            if (piecePlacement.Count(c => c == 'k') != 1)
                throw new IllegalFenException("There must be exactly one black King!");

            if (piecePlacement.Count(c => c == 'K') != 1)
                throw new IllegalFenException("There must be exactly one white King!");

            // FEN is from rank 8 to rank 1, but we store it from rank 1 to rank 8
            var ranks = piecePlacement.Split('/').Reverse().ToArray();

            if (ranks.Length != 8)
                throw new IllegalFenException($"The format must describe 8 ranks, got {ranks.Length} instead.");

            var pieces = new List<ChessPiece>();
            for (var rank = 0; rank < 8; rank++)
            {
                var rankString = ranks[rank];

                for (var file = 0; file < 8; file++)
                {
                    var piece = rankString[file];

                    if (piece is >= '1' and <= '8')
                    {
                        file += piece - '0' - 1;
                        continue;
                    }

                    var color = char.IsUpper(piece) ? Color.White : Color.Black;
                    var pieceType = char.ToLower(piece) switch
                    {
                        'p' => PieceType.Pawn,
                        'n' => PieceType.Knight,
                        'b' => PieceType.Bishop,
                        'r' => PieceType.Rook,
                        'q' => PieceType.Queen,
                        'k' => PieceType.King,
                        _ => throw new IllegalFenException($"Piece type must be one of p, n, b, r, q, k, instead is {piece}")
                    };
                    pieces.Add(new ChessPiece(color, pieceType, new Square(file, rank)));
                }
            }

            var currentTurn = components[1] == "w" ? Color.White : (components[1] == "b" ? Color.Black : throw new IllegalFenException($"Piece color must be either black or white, instead is {components[1]}"));

            var castlingAvailability = components[2];
            if (castlingAvailability.Any(c => c != '-' && c != 'K' && c != 'k' && c != 'Q' && c != 'q'))
                throw new IllegalFenException($"Castling availability must be one of -, K, k, Q, q, instead is {castlingAvailability}");

            var castlingRights = CastlingRights.None;
            castlingRights |= castlingAvailability.Contains('K') ? CastlingRights.WhiteKingSide : CastlingRights.None;
            castlingRights |= castlingAvailability.Contains('Q') ? CastlingRights.WhiteQueenSide : CastlingRights.None;
            castlingRights |= castlingAvailability.Contains('k') ? CastlingRights.BlackKingSide : CastlingRights.None;
            castlingRights |= castlingAvailability.Contains('q') ? CastlingRights.BlackQueenSide : CastlingRights.None;

            Square? enPassantTargetSquare = null;
            var enPassantTargetSquareString = components[3];
            if (enPassantTargetSquareString != "-")
            {
                var file = enPassantTargetSquareString[0] - 'a';
                var rank = enPassantTargetSquareString[1] - '1';
                enPassantTargetSquare = new Square(file, rank);
            }

            var halfMoveClock = 0;
            try
            {
                halfMoveClock = Convert.ToInt32(components[4]);
            }
            catch (FormatException e)
            {
                throw new IllegalFenException($"Half move clock must be an integer, instead is {components[4]}", e);
            }

            var fullMoveNumber = 0;
            try
            {
                fullMoveNumber = Convert.ToInt32(components[5]);
            }
            catch (FormatException e)
            {
                throw new IllegalFenException($"Full move number must be an integer, instead is {components[5]}", e);
            }

            return new ChessBoard(currentTurn, halfMoveClock, fullMoveNumber, pieces, castlingRights, enPassantTargetSquare);
        }
    }
}
