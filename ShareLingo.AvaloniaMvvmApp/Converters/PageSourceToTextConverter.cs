using CONTENT = ShareLingo.Core.Resources.Content;
using Avalonia.Data.Converters;
using ShareLingo.Core.ViewModel.Component;
using System;
using System.Globalization;

namespace ShareLingo.AvaloniaMvvmApp.Converters
{
    public class PageSourceToTextConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not PageSource source) return null!;
            switch (source)
            {
                case PageSource.CourseBrowser: return CONTENT.mainWindow_courseBrowserNavigationMenuItem;
                default: throw new NotImplementedException($"There is no implmenentation for: {source}.");
            }
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}