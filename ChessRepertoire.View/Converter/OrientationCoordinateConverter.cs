using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ChessRepertoire.Model.Board;

namespace ChessRepertoire.View.Wpf.Converter;

/// <summary>
/// Returns the coordinate of a field in the specified orientation.
/// </summary>
public class OrientationCoordinateConverter : IMultiValueConverter {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
        if (values.Length != 2 || !typeof(double).IsAssignableFrom(targetType)) {
            throw new ArgumentException("Invalid arguments");
        }

        if (values[0] is int coordinate && values[1] is Color color) {
            return color switch {
                Color.White => (double) coordinate,
                Color.Black => (double) 7 - coordinate,
                _ => throw new ArgumentException("Invalid parameter", nameof(parameter))
            };
        }

        return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
        return null;
    }
}
