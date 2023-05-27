using System.Reactive.Disposables;
using ChessRepertoire.ViewModel.Board;
using ReactiveUI;

namespace ChessRepertoire.View.Wpf.Control.Board {
    /// <summary>
    /// Interaction logic for ChessBoard.xaml
    /// </summary>
    public partial class ChessBoard : ReactiveUserControl<IBoardViewModel> {
        public ChessBoard() {
            InitializeComponent();

            ViewModel = new BoardViewModel();

            this.WhenActivated(d => {
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Fields,
                        view => view.Fields.ItemsSource)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Pieces,
                        view => view.Pieces.ItemsSource)
                    .DisposeWith(d);
            });
        }
    }
}
