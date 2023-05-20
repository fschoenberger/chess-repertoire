using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessRepertoire.ViewModel.Board;
using ReactiveUI;

namespace ChessRepertoire.View.Wpf.Control.Board {
    /// <summary>
    /// Interaction logic for Board.xaml
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
            });
        }
    }
}
