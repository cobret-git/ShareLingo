using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace ShareLingo.WinUI.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool boolean) return null!;
            return boolean
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is not Visibility visibility) return null!;
            switch (visibility)
            {
                case Visibility.Visible: return false;
                case Visibility.Collapsed: return true;
                default: return null!;
            }
        }
    }
}
