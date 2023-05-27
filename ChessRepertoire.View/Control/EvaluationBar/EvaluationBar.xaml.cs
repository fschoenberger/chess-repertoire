using System;
using System.Collections.Generic;
using System.Linq;
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
using ChessRepertoire.ViewModel.EvaluationBar;
using ReactiveUI;

namespace ChessRepertoire.View.Wpf.Control.EvaluationBar {
    /// <summary>
    /// Interaction logic for EvaluationBar.xaml
    /// </summary>
    public partial class EvaluationBar : ReactiveUserControl<IEvaluationBarViewModel> {
        public EvaluationBar() {
            InitializeComponent();
        }
    }
}
