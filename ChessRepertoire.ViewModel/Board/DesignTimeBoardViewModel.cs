using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessRepertoire.Model.Piece;
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
