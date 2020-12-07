using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Proggy.Infrastructure.Converters
{
    public class ColorToSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = (SolidColorBrush)value;

            return brush.Color;
        }
    }
}
