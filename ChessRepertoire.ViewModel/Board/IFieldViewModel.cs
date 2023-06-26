using ReactiveUI;

namespace ChessRepertoire.ViewModel.Board;

public interface IFieldViewModel {
    public int Rank { get; set; }

    public int File { get; set; }

    //public bool IsTarget { get; set; }
}