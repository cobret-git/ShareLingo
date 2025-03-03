using Microsoft.UI.Xaml.Data;
using System;

namespace ShareLingo.WinUI.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool boolean) return null!;
            return !boolean;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool boolean) return null!;
            return !boolean;
        }
    }
}
