using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShareLingo.WPF.Components.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            throw new NotSupportedException("NullToVisibilityConverter does not support ConvertBack.");
        }
    }
}
