using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using ChessRepertoire.ViewModel.Board;
using ReactiveMarbles.ObservableEvents;
using System.Windows.Controls;
using System.Windows.Shapes;
using ReactiveUI;

namespace ChessRepertoire.View.Wpf.Control.Board
{
    public partial class ChessBoard : ReactiveUserControl<IBoardViewModel>
    {
        public ChessBoard()
        {
            InitializeComponent();

            ViewModel = new BoardViewModel();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Fields,
                        view => view.Fields.ItemsSource)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Pieces,
                        view => view.Pieces.ItemsSource)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.FlipBoard, v => v.FlipButton).DisposeWith(d);

                Fields.Events().MouseDown.Subscribe((e) => {
                    if (e.OriginalSource is Border { DataContext: IFieldViewModel field })
                    {
                        Debug.WriteLine($"Click on {(char)('A' + field.File)}{field.Rank + 1}");
                    }
                }).DisposeWith(d);

                Pieces.Events().MouseDown.Subscribe((e) => {
                    if (e.OriginalSource is Path { DataContext: IPieceViewModel piece })
                    {
                        Debug.WriteLine($"Click on {piece.Color} {piece.Type} at {(char)('A' + piece.File)}{piece.Rank + 1}");
                    }
                }).DisposeWith(d);
            });
        }
    }
}
