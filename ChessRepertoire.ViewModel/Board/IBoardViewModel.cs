using System.Collections.ObjectModel;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.Board;

public interface IBoardViewModel : IReactiveObject
{
    public IEnumerable<IFieldViewModel> Fields { get; }

    public ReadOnlyObservableCollection<IPieceViewModel> Pieces { get; }
}