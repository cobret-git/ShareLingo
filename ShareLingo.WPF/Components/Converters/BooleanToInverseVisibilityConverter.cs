using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShareLingo.WPF.Components.Converters
{
    public class BooleanToInverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            bool flag = value is bool && (bool)value;
            return flag ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }
            return false;
        }
    }
}
