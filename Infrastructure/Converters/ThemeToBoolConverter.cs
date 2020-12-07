using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Material.Styles.Themes.Base;

namespace Proggy.Infrastructure.Converters
{
    public class ThemeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (BaseThemeMode)value == (BaseThemeMode)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
