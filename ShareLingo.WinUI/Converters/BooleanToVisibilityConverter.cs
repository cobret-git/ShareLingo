using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLingo.WinUI.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool boolean) return null!;
            return boolean
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is not Visibility visibility) return null!;
            switch (visibility)
            {
                case Visibility.Visible: return true;
                case Visibility.Collapsed: return false;
                default: return null!;
            }
        }
    }
}
