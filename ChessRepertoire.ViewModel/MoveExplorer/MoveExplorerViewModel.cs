using System.Collections.ObjectModel;
using ChessRepertoire.Model.Board;
using ChessRepertoire.ViewModel.Util;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.MoveExplorer {
    public sealed class MoveExplorerViewModel : ReactiveObject, IDisposable {
        private readonly ReadOnlyObservableCollection<MoveViewModel> _moves;
        public ReadOnlyObservableCollection<MoveViewModel> Moves => _moves;

        private readonly IDisposable _dispose;

        public MoveExplorerViewModel(ChessGame game) {
            var service = new MoveService();

            _dispose = service.Moves.Connect()
                .Bind(out _moves)
                .Subscribe();
        }

        public void Dispose() {
            _dispose.Dispose();
        }
    }
}
