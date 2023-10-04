using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChessRepertoire.Model.Board;
using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.MoveExplorer {
    public sealed class MoveExplorerViewModel : ReactiveObject, IDisposable {
        private readonly ReadOnlyObservableCollection<MoveViewModel> _moves;
        public ReadOnlyObservableCollection<MoveViewModel> Moves => _moves;

        private readonly IDisposable _dispose;

        public MoveExplorerViewModel(ChessGame game) {
            var locker = new object();

            var predecessors = game.Edges.Connect().Synchronize(locker)
                .Group(x => x.To, game.BoardStates.Connect().DistinctValues(x => x.Id))
                .AsObservableCache();


            _dispose = predecessors.Connect()
                .Filter(x => x.Cache.Count == 0)
                .TransformMany(group => group.Cache, e => e.Id)
                .Transform(edge => new MoveViewModel(edge, game.Edges.Connect()))
                .Bind(out _moves)
                .DisposeMany()
                .Subscribe();
        }

        public void Dispose() {
            _dispose.Dispose();
        }
    }

    public class MoveViewModel : IDisposable {
        private readonly ReadOnlyObservableCollection<MoveViewModel> _children;
        public ReadOnlyObservableCollection<MoveViewModel> Children => _children;

        private readonly IDisposable _dispose;

        public int Depth { get; init; }
        public string Text { get; init; }

        public MoveViewModel(Edge e, IObservable<IChangeSet<Edge, UInt128>> edges, int depth = 1) {
            Depth = depth;
            Text = e.Move;

            _dispose = edges
                    .Filter(edge => e.To == edge.From)
                    .Transform(edge => new MoveViewModel(edge, edges, depth + 1))
                    .Bind(out _children)
                    .Subscribe();
        }


        public void Dispose() {
            _dispose.Dispose();
        }
    }
}
