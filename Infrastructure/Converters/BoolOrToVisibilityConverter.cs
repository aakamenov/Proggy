using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Proggy.Infrastructure.Converters
{
    public class BoolOrToVisibilityConverter : BoolToVisibilityConverterBase, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //https://github.com/dotnet/wpf/issues/1706
            if (values.Any(x => x == DependencyProperty.UnsetValue))
                return Visibility.Collapsed;

            var val = values.Cast<bool>().Any(x => x == true);

            return ConvertVisibility(val, parameter);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
