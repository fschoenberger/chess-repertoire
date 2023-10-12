using System.Collections.ObjectModel;
using ChessRepertoire.Model.Board;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.MoveExplorer {
    public sealed class MoveExplorerViewModel : ReactiveObject, IDisposable {
        private readonly ReadOnlyObservableCollection<MoveViewModel> _moves;
        public ReadOnlyObservableCollection<MoveViewModel> Moves => _moves;

        private readonly IDisposable _dispose;

        public MoveExplorerViewModel(ChessGame game) {
            _dispose = game.RootStates.Connect()
                .MergeMany(s => s.Children.Connect())
                .Transform(edge => new MoveViewModel(edge))
                .Bind(out _moves)
                .DisposeMany()
                .Subscribe();
        }

        public void Dispose() {
            _dispose.Dispose();
        }
    }

    public sealed class MoveViewModel : IDisposable {
        private readonly ReadOnlyObservableCollection<MoveViewModel> _children;
        public ReadOnlyObservableCollection<MoveViewModel> Children => _children;

        private readonly IDisposable _dispose;

        public int Depth { get; init; }
        public string Text { get; init; }

        public MoveViewModel(Edge e) {
            Depth = e.From.FullMoveNumber;
            Text = e.Move.ToString();

            _dispose = e.To.Children.Connect()
                    .Transform(edge => new MoveViewModel(edge))
                    .Bind(out _children)
                    .Subscribe();
        }


        public void Dispose() {
            _dispose.Dispose();
        }
    }
}
