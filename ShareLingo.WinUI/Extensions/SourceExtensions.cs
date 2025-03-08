using NetForge.Core;
using ShareLingo.WinUI.ViewModel.Component;
using System.Reflection;
using System.Reflection.Metadata;

namespace ShareLingo.WinUI.Extensions
{
    public static class SourceExtensions
    {
        public static PageNavigationRequest ToRequest(this PageSource source, 
            NavigationRequestAction action, IPageDataParameter? parameter = null)
        {
            var field = source.GetType().GetField(source.ToString());
            var attribute = field?.GetCustomAttribute<PageSourceAttribute>();
            if (attribute == null) return null!;
            return new PageNavigationRequest(attribute.ViewModelType, action, parameter);

        }
    }
}
