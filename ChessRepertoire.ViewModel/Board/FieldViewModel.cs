using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ChessRepertoire.ViewModel.Board {
    internal class FieldViewModel : IFieldViewModel
    {
        public int Rank { get; set; }

        public int File { get; set; }

        //[Reactive] public bool IsTarget { get; set; } = false;
    }
}
