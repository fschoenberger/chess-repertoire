using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

                Fields.Events().MouseDown
                    .Where(e => e.OriginalSource is Border { DataContext: IFieldViewModel field })
                    .Select(e => ((Border)e.OriginalSource).DataContext as IFieldViewModel)
                    .WhereNotNull()
                    .InvokeCommand(ViewModel, vm => vm.SelectField)
                    .DisposeWith(d);

                Pieces.Events().MouseDown
                    .Where(e => e.OriginalSource is Path { DataContext: IPieceViewModel piece })
                    .Select(e => ((Path)e.OriginalSource).DataContext as IPieceViewModel)
                    .WhereNotNull()
                    .InvokeCommand(ViewModel, vm => vm.SelectPiece)
                    .DisposeWith(d);
            });
        }
    }
}
