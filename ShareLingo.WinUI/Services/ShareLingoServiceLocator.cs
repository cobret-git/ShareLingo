using NetForge.Core;
using ShareLingo.Core.ViewModel;
using ShareLingo.WinUI.View;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ShareLingo.WinUI.Services
{
    public class ShareLingoServiceLocator : IServiceLocator
    {
        #region Fields
        private readonly Dictionary<Type, Type> viewModelsToView = new()
        {
            {typeof(CourseBrowserViewModel), typeof(CourseBrowser) },
            {typeof(CourseEditorViewModel), typeof(CourseEditor) },
            {typeof(CourseViewerViewModel), typeof(CourseViewer) },
        };
        #endregion

        public object? GetService(Type serviceType)
        {
            return App.Current.Services.GetService(serviceType);
        }
        public Type GetViewType(Type viewModelType)
        {
            return viewModelsToView[viewModelType];
        }
        public bool TryGetViewType(Type viewModelType, [MaybeNullWhen(false)] out Type viewType)
        {
            return viewModelsToView.TryGetValue(viewModelType, out viewType);
        }
    }
}
