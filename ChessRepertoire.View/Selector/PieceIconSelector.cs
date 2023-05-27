using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ChessRepertoire.Model.Board;
using ChessRepertoire.ViewModel.Board;

namespace ChessRepertoire.View.Wpf.Selector {
    internal class PieceIconSelector : DataTemplateSelector {
        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            if (item is IPieceViewModel piece) {
                var ret = Application.Current.FindResource(piece.Type + "" + piece.Color) as DataTemplate;
                return ret;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
