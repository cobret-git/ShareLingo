using ShareLingo.Core.ViewModel;
using ShareLingo.Wpf.View;
using System.Globalization;
using System.Windows.Data;

namespace ShareLingo.Wpf.Converters
{
    public class ViewModelToViewConverter : IValueConverter
    {
        private readonly Dictionary<Type, Type> viewTypes = new Dictionary<Type, Type>()
        {
            {typeof(CourseBrowserViewModel), typeof(CourseBrower) }
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Type viewModelType || !viewTypes.ContainsKey(viewModelType)) return null!;
            return viewTypes[viewModelType];
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
