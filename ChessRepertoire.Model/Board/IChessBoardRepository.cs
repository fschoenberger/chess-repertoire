using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessRepertoire.Model.Board
{
    public interface IChessBoardRepository {
        ChessGame FromFen(string fen);
    }
}
