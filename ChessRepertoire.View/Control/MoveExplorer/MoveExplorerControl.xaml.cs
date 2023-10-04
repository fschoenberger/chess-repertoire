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
using ChessRepertoire.ViewModel.MoveExplorer;
using ReactiveUI;

namespace ChessRepertoire.View.Wpf.Control.MoveExplorer {
    public partial class MoveExplorerControl : ReactiveUserControl<MoveExplorerViewModel> {
        public MoveExplorerControl() {
            InitializeComponent();

            this.WhenActivated(d => {
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Moves,
                        view => view.MoveTree.ItemsSource)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.Moves.Count,
                    v => v.MoveCounter.Text)
                    .DisposeWith(d);
            });
        }
    }
}
