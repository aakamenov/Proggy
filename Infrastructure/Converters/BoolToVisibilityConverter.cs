using System;
using System.Globalization;
using System.Windows.Data;

namespace Proggy.Infrastructure.Converters
{
    class BoolToVisibilityConverter : BoolToVisibilityConverterBase, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertVisibility(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
