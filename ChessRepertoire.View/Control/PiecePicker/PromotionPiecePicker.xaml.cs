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
using ChessRepertoire.Model.Board;

namespace ChessRepertoire.View.Wpf.Control.PiecePicker
{
    /// <summary>
    /// Interaction logic for PromotionPiecePicker.xaml
    /// </summary>
    public partial class PromotionPiecePicker : Window {
        public PieceType Type { get; private set; } = PieceType.Queen;

        public PromotionPiecePicker()
        {
            InitializeComponent();
        }

        private void BishopButton_Click(object sender, RoutedEventArgs e) {
            Type = PieceType.Bishop;
            Close();
        }

        private void KnightButton_Click(object sender, RoutedEventArgs e) {
            Type = PieceType.Knight;
            Close();
        }

        private void RookButton_Click(object sender, RoutedEventArgs e) {
            Type = PieceType.Rook;
            Close();
        }

        private void QueenButton_Click(object sender, RoutedEventArgs e) {
            Type = PieceType.Queen;
            Close();
        }
    }
}
