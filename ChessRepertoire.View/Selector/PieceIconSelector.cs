using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ChessRepertoire.Model.Piece;

namespace ChessRepertoire.View.Wpf.Selector {
    internal class PieceIconSelector : DataTemplateSelector, IPieceVisitor<string> {
        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            if (item is IPieceVisitable piece) {
                var ret = Application.Current.FindResource(piece.Accept(this)) as DataTemplate;
                return ret;
            }

            return base.SelectTemplate(item, container);
        }

        public string Visit(Pawn pawn) {
            return "Pawn" + pawn.Color;
        }

        public string Visit(Knight knight) {
            return "Knight" + knight.Color;
        }

        public string Visit(Bishop bishop) {
            return "Bishop" + bishop.Color;
        }

        public string Visit(Rook rook) {
            return "Rook" + rook.Color;
        }

        public string Visit(Queen queen) {
            return "Queen" + queen.Color;
        }

        public string Visit(King king) {
            return "King" + king.Color;
        }
    }
}
