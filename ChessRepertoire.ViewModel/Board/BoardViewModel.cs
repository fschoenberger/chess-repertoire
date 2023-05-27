using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChessRepertoire.Model.Board;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ChessRepertoire.ViewModel.Board {
    public class BoardViewModel : ReactiveObject, IBoardViewModel {
        private readonly IFieldViewModel[,] _fields;
        public IEnumerable<IFieldViewModel> Fields {
            get {
                var rowCount = _fields.GetLength(0);
                var columnCount = _fields.GetLength(1);

                return
                    from row in Enumerable.Range(0, rowCount)
                    from column in Enumerable.Range(0, columnCount)
                    select _fields[row, column];
            }
        }

        private readonly ChessBoard _board;

        private readonly ReadOnlyObservableCollection<IPieceViewModel> _pieces;
        public ReadOnlyObservableCollection<IPieceViewModel> Pieces => _pieces;

        [Reactive] public Color Orientation { get; set; } = Color.Black;

        public ICommand FlipBoard { get; }

        public BoardViewModel() {
            _board = new ChessBoard();

            _fields = new IFieldViewModel[8, 8];

            for (var i = 0; i < 8; ++i) {
                for (var j = 0; j < 8; ++j) {
                    _fields[i, j] = new FieldViewModel { Row = i, Column = j };
                }
            }

            FlipBoard = ReactiveCommand.Create(() => {
                Orientation = Orientation == Color.Black ? Color.White : Color.Black;
            });

            _board.Pieces.Connect()
                .Transform(p => (IPieceViewModel) new PieceViewModel(p))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _pieces)
                .Subscribe();
        }
    }
}
