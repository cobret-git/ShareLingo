using System.Diagnostics.CodeAnalysis;
using NetForge.Core;
using ShareLingo.Core.ViewModel;
using ShareLingo.WPF.View;

namespace ShareLingo.WPF.Services;

public class ServiceLocator : IServiceLocator
{
    private readonly Dictionary<Type, Type> viewModelTypeMap = new Dictionary<Type, Type>()
    {
        { typeof(CourseBrowserViewModel), typeof(CourseBrowser) },
        { typeof(CourseEditorViewModel), typeof(CourseEditor) },
    };

    public object? GetService(Type serviceType)
    {
        return App.Current.Services.GetService(serviceType);
    }

    public Type GetViewType(Type viewModelType)
    {
        if (!viewModelTypeMap.ContainsKey(viewModelType))
            throw new NotImplementedException();
        return viewModelTypeMap[viewModelType];
    }

    public bool TryGetViewType(Type viewModelType, [MaybeNullWhen(false)] out Type viewType)
    {
        if (viewModelTypeMap.ContainsKey(viewModelType))
        {
            viewType = viewModelTypeMap[viewModelType];
            return true;
        }

        viewType = null;
        return false;
    }
}