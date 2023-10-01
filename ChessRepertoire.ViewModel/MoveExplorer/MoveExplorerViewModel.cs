using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChessRepertoire.Model.Board;
using DynamicData;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.MoveExplorer {
    public class MoveExplorerViewModel : ReactiveObject {
        private readonly ObservableCollection<MoveViewModel> _moves;
        public ObservableCollection<MoveViewModel> Moves => _moves;

        public MoveExplorerViewModel(ChessGame game) {
            //game.BoardStates
            //    .Connect(x => x.Parents.Count == 0)
            //    .Transform(x => new MoveViewModel())
            //    .Bind(ref _moves);

        }
    }

    public class MoveViewModel {

    }
}
