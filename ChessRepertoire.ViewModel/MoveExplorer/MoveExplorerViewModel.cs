using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChessRepertoire.Model.Board;
using DynamicData;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.MoveExplorer {
    public class MoveExplorerViewModel : ReactiveObject {
        private readonly ObservableCollection<dynamic> _moves;
        public ObservableCollection<dynamic> Moves => _moves;

        public MoveExplorerViewModel(ChessBoard board) {
            Observable.Create<IChangeSet<dynamic, ulong>>(
                observer => {
                    var locker = new object();


                    return Disposable.Empty;
                });

            _moves = new ObservableCollection<dynamic> { };
        }
    }

    public class MoveViewModel {

    }
}
