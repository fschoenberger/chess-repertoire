using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessRepertoire.Model.Board;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ChessRepertoire.ViewModel.MoveExplorer
{
    public class MoveExplorerViewModel : ReactiveObject {
        private readonly ObservableCollection<MoveViewModel> _moves;
        public ObservableCollection<MoveViewModel> Moves => _moves;

        public MoveExplorerViewModel(ChessBoard board)
        {
            _moves = new ObservableCollection<MoveViewModel>{
                new() { FullMoveNumber = 1 },
                new() { FullMoveNumber = 2 },
                new() { FullMoveNumber = 3},
                new() { FullMoveNumber = 4, Children = new List<MoveViewModel>{
                    new() { FullMoveNumber = 5 },
                    new() { FullMoveNumber = 6 },
                    new() { FullMoveNumber = 7 },
                }}
            };

            Debug.WriteLine("Done.");
        }
    }
}
