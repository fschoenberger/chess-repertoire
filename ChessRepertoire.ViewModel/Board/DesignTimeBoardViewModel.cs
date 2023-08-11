using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChessRepertoire.Model.Board;
using ChessRepertoire.ViewModel.Util;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.Board {
    public class DesignTimeBoardViewModel : ReactiveObject, IBoardViewModel {
        public IEnumerable<IFieldViewModel> Fields {
            get {
                for (int i = 0; i < 8; ++i)
                    for (int j = 0; j < 8; ++j)
                        yield return new FieldViewModel { Rank = i, File = j };
            }
        }

        private IList<IPieceViewModel> _pieces = new List<IPieceViewModel>
        {
            new PieceViewModel(PieceType.Pawn, Color.White, 1, 1),
            new PieceViewModel(PieceType.Pawn, Color.White, 1, 2),
            new PieceViewModel(PieceType.Pawn, Color.White, 1, 3),
            new PieceViewModel(PieceType.Pawn, Color.White, 1, 4),
            new PieceViewModel(PieceType.Pawn, Color.White, 1, 5),
            new PieceViewModel(PieceType.Pawn, Color.White, 1, 6),
            new PieceViewModel(PieceType.Pawn, Color.White, 1, 7),
        };


        public IEnumerable<IPieceViewModel> Pieces => _pieces;

        public Color Orientation { get; set; } = Color.Black;

        public ICommand FlipBoardCommand { get; } = new NullCommand();

        public ICommand SelectFieldCommand { get; } = new NullCommand();

        public ICommand SelectPieceCommand { get; } = new NullCommand();

        public Interaction<Unit, PieceType> Promotion { get; }
    }
}
