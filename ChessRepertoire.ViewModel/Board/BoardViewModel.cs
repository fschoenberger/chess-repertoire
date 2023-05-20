using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessRepertoire.Model.Piece;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.Board {
    public class BoardViewModel : ReactiveObject, IBoardViewModel {
        private IFieldViewModel[,] _fields;

        public BoardViewModel() {
            _fields = new IFieldViewModel[8, 8];

            for (var i = 0; i < 8; ++i) {
                for (var j = 0; j < 8; ++j) {
                    _fields[i, j] = new FieldViewModel { Row = i, Column = j };
                }
            }
        }

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

        public ObservableCollection<IPieceViewModel> Pieces { get; } = new(new List<IPieceViewModel>
        {
            new PieceViewModel { Piece = new Pawn(Color.White), Row = 1, Column = 0 },
            new PieceViewModel { Piece = new Pawn(Color.White), Row = 1, Column = 1 },
            new PieceViewModel { Piece = new Pawn(Color.White), Row = 1, Column = 2 },
            new PieceViewModel { Piece = new Pawn(Color.White), Row = 1, Column = 3 },
            new PieceViewModel { Piece = new Pawn(Color.White), Row = 1, Column = 4 },
            new PieceViewModel { Piece = new Pawn(Color.White), Row = 1, Column = 5 },
            new PieceViewModel { Piece = new Pawn(Color.White), Row = 1, Column = 6 },
            new PieceViewModel { Piece = new Pawn(Color.White), Row = 1, Column = 7 },
        });

    }
}
