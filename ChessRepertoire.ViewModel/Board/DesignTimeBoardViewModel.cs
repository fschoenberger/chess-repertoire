using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessRepertoire.Model.Board;
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

        private ObservableCollection<IPieceViewModel> _pieces = new(new List<IPieceViewModel>
        {
            new PieceViewModel(PieceType.Pawn, Color.White, 1, 1),
        });


        public ReadOnlyObservableCollection<IPieceViewModel> Pieces => new(_pieces);
    }
}
