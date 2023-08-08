using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using ChessRepertoire.Model.Board;
using ReactiveUI;

namespace ChessRepertoire.ViewModel.Board;

public interface IBoardViewModel : IReactiveObject
{
    public IEnumerable<IFieldViewModel> Fields { get; }

    public ReadOnlyObservableCollection<IPieceViewModel> Pieces { get; }

    public Color Orientation { get; set; }

    public ICommand FlipBoardCommand { get; }

    public ICommand SelectFieldCommand { get; }

    public ICommand SelectPieceCommand { get; }

    Interaction<Unit, PieceType> Promotion { get; }
}