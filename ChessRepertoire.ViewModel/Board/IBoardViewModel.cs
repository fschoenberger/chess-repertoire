using ReactiveUI;

namespace ChessRepertoire.ViewModel.Board;

public interface IBoardViewModel : IReactiveObject
{
    public IEnumerable<IFieldViewModel> Fields { get; }
}