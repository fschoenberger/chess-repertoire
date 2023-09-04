using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.MoveExplorer
{
    public class MoveViewModel : ReactiveObject
    {
        public int FullMoveNumber { get; set; }

        public string WhiteMoveSan { get; set; }

        public string BlackMoveSan { get; set; }

        public string WhiteMoveComment { get; set; }

        public string BlackMoveComment { get; set; }

        public IEnumerable<MoveViewModel> Children { get; set; }
    }
}
