using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NetForge.Core;
using NetForge.WinUI;
using ShareLingo.Core.Services;
using ShareLingo.Core.ViewModel;
using ShareLingo.WinUI.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ShareLingo.WinUI.View
{
    public sealed partial class MainWindow : Window
    {
        #region Fields
        private readonly IEventAggregator eventAggregator;
        private readonly IServiceLocator serviceLocator;
        private readonly Dictionary<Type, int> subscribeTokens = new();
        private readonly Dictionary<Type, Type> viewModelsToView = new();
        #endregion

        #region Constructors
        public MainWindow()
        {
            this.eventAggregator = App.Current.Services.GetService<IEventAggregator>()!;
            this.serviceLocator = App.Current.Services.GetService<IServiceLocator>()!;

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);

            this.InitializeComponent();
            var filePicker = App.Current.Services.GetService<IBuildInfoManager>()?.FilePicker as FilePicker;
            if (filePicker != null) filePicker.MainWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            RootGrid.DataContext = this;

            subscribeTokens.Add(typeof(PageNavigationRequest),
                eventAggregator.SubscribeAction<PageNavigationRequest>(OnPageNavigationRequestReceived));
        }
        #endregion

        #region Properties
        public ObservableCollection<BreadcrumbBarDataItem> BreadcrumbItems { get; } = new();
        #endregion

        #region Methods
        public void NavigateToNext(Type viewModelType, IViewModelDataParameter? dataParameter = null)
        {
            try
            {
                if (contentFrame == null) throw new ArgumentNullException(nameof(contentFrame));
                else if (viewModelType == null) throw new ArgumentNullException(nameof(viewModelType));
                if (!serviceLocator.TryGetViewType(viewModelType, out var viewType)) 
                    throw new NotSupportedException($"ViewModel type {viewModelType.Name} is not supported.");
                if (BreadcrumbItems.LastOrDefault()?.AssociatedPage != null)
                    if (!ClosePage(BreadcrumbItems.Last().AssociatedPage)) return;
                contentFrame.Navigate(viewType, dataParameter);
                var pageViewModel = (contentFrame.Content as Page)?.DataContext as IPageViewModel;
                if (pageViewModel == null) throw new ArgumentNullException(nameof(pageViewModel));
                pageViewModel.DataParameter = dataParameter;
                var breadcrumbItem = new BreadcrumbBarDataItem(pageViewModel);
                eventAggregator.InvokeActionOnUIThread(() => BreadcrumbItems.Add(breadcrumbItem));
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        public void NavigateToPrevious(Type viewModelType, IViewModelDataParameter? dataParameter = null)
        {
            try
            {
                if (contentFrame == null) throw new ArgumentNullException(nameof(contentFrame));
                else if (viewModelType == null) throw new ArgumentNullException(nameof(viewModelType));
                var item = BreadcrumbItems.FirstOrDefault(x =>
                    x.AssociatedPage?.GetType() == viewModelType
                    && (x.AssociatedPage.DataParameter == null || x.AssociatedPage.DataParameter.Equals(dataParameter)));
                if (item == null) throw new ArgumentNullException(nameof(item));
                NavigateToPrevious(item);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        public void NavigateToPrevious(BreadcrumbBarDataItem item)
        {
            try
            {
                if (BreadcrumbItems.Contains(item) != true) return;
                var itemIndex = BreadcrumbItems.IndexOf(item);
                var itemsToClose = BreadcrumbItems.Skip(Math.Max(1, itemIndex)).Reverse().ToArray();
                foreach (var itemToDispose in itemsToClose)
                {
                    if (!ClosePage(itemToDispose.AssociatedPage)) break;
                    BreadcrumbItems.Remove(itemToDispose);
                }
                var viewModelType = BreadcrumbItems.Last().AssociatedPage.GetType();
                var dataParameter = BreadcrumbItems.Last().AssociatedPage.DataParameter;
                if (!serviceLocator.TryGetViewType(viewModelType, out var viewType))
                    throw new NotSupportedException($"ViewModel type {viewModelType.Name} is not supported.");
                contentFrame.Navigate(viewType, dataParameter);
                var pageViewModel = (contentFrame.Content as Page)?.DataContext as IPageViewModel;
                if (pageViewModel == null) throw new ArgumentNullException(nameof(pageViewModel));
                pageViewModel.DataParameter = BreadcrumbItems.Last().AssociatedPage.DataParameter;
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        public bool ClosePage(IPageViewModel viewModel)
        {
            var closingArgs = new PageClosingEventArgs(this, viewModel);
            eventAggregator.Publish(closingArgs);
            if (closingArgs.IsCanceled) return false;
            var closedArgs = new PageClosedEventArgs(this, viewModel);
            eventAggregator.Publish(closedArgs);
            if (!viewModel.IsDisposed) viewModel.Dispose();
            return true;
        }
        #endregion

        #region Handlers
        private void nvSample_Loaded(object sender, RoutedEventArgs e)
        {
            var msgMng = App.Current.Services.GetService<IMessageManager>() as ContentDialogHost;
            if (msgMng != null) msgMng.XamlRoot = this.Content.XamlRoot;
            OnPageNavigationRequestReceived(new PageNavigationRequest(typeof(CourseBrowserViewModel), NavigationRequestAction.GoNext));
        }
        private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is not BreadcrumbBarDataItem selectedItem) return;
            NavigateToPrevious(selectedItem);
        }
        private void OnPageNavigationRequestReceived(PageNavigationRequest request)
        {
            if (request == null) return;
            switch (request.Action)
            {
                case NavigationRequestAction.GoNext: NavigateToNext(request.ViewModelType, request.DataParameter); break;
                case NavigationRequestAction.GoBack: NavigateToPrevious(request.ViewModelType, request.DataParameter); break;
                case NavigationRequestAction.ToRoot:
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
