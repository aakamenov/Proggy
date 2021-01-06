using System.Windows;

namespace Proggy.Infrastructure.Converters
{
    public abstract class BoolToVisibilityConverterBase
    {
        protected object ConvertVisibility(object value, object parameter)
        {
            if (!bool.TryParse(parameter as string, out bool negate))
                negate = false;

            bool val = negate ^ (bool)value;

            return val ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
