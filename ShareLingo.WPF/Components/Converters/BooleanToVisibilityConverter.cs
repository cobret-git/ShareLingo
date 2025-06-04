using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShareLingo.WPF.Components.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            bool flag = false;
            if (value is bool)
                flag = (bool)value;

            if (flag) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value is Visibility)
            {
                if ((Visibility)value == Visibility.Visible)
                    return true;
                else return false;
            }
            return false;
        }
    }
}
