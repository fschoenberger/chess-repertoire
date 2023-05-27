using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ChessRepertoire.View.Wpf.Converter {
    public class FieldBrushConverter : IMultiValueConverter {
        private readonly Brush _whiteBrush = Application.Current.FindResource("LightSquareBrush") as Brush ?? new SolidColorBrush(Colors.NavajoWhite);

        private readonly Brush _blackBrush = Application.Current.FindResource("DarkSquareBrush") as Brush ?? new SolidColorBrush(Colors.Peru);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values.Length != 2 || !typeof(Brush).IsAssignableFrom(targetType)) {
                throw new ArgumentException("Invalid arguments");
            }

            if (values[0] is int row && values[1] is int column) {
                return (row + column) % 2 == 0 ? _whiteBrush : _blackBrush;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
