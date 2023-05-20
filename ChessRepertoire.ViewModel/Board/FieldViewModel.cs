using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessRepertoire.ViewModel.Board {
    internal class FieldViewModel : IFieldViewModel
    {
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
