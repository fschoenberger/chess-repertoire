using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.Board {
    public class DesignTimeBoardViewModel : ReactiveObject, IBoardViewModel {
        public IEnumerable<IFieldViewModel> Fields {
            get {
                for (int i = 0; i < 8; ++i)
                    for (int j = 0; j < 8; ++j)
                        yield return new FieldViewModel { Row = i, Column = j };
            }
        }
    }
}
