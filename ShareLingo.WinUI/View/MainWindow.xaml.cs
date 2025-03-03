using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NetForge.Core;
using NetForge.Core.EventArgs;
using ShareLingo.WinUI.ViewModel;
using ShareLingo.WinUI.ViewModel.Component;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ShareLingo.WinUI.View
{
    public sealed partial class MainWindow : Window
    {
        #region Fields
        private readonly IEventAggregator eventAggregator;
        private readonly Dictionary<Type, int> subscribeTokens = new();
        private readonly Dictionary<Type, Type> viewModelsToView = new();
        #endregion

        #region Constructors
        public MainWindow()
        {
            this.eventAggregator = App.Current.Services.GetService<IEventAggregator>()!;
            this.ExtendsContentIntoTitleBar = true;
            this.InitializeComponent();
            RootGrid.DataContext = this;

            subscribeTokens.Add(typeof(PageNavigationRequest),
                eventAggregator.SubscribeAction<PageNavigationRequest>(OnPageNavigationRequestReceived));
        }
        #endregion

        #region Properties
        public ObservableCollection<BreadcrumbBarDataItem> BreadcrumbItems { get; } = new();
        #endregion

        #region Methods
        public void NavigateToNext(IPageViewModel viewModel, IPageDataParameter? dataParameter = null)
        {
            try
            {
                if (contentFrame == null) throw new ArgumentNullException(nameof(contentFrame));
                else if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
                var dictContainsKey = viewModelsToView.ContainsKey(viewModel.GetType());
                if (!dictContainsKey) throw new ArgumentException($"Passed viewModel's type was not implemented: {viewModel.GetType().Name}");
                var breadcrumbItem = new BreadcrumbBarDataItem(viewModel);
                BreadcrumbItems.Add(breadcrumbItem);
                var view = Activator.CreateInstance(viewModelsToView[viewModel.GetType()]) as Page;
                if (view == null) throw new ArgumentNullException(nameof(view));
                view.DataContext = viewModel;
                contentFrame.Navigate(view.GetType(), dataParameter);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        public void NavigateToPrevious(IPageViewModel viewModel, IPageDataParameter? dataParameter = null)
        {
            try
            {
                if (contentFrame == null) throw new ArgumentNullException(nameof(contentFrame));
                else if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
                var item = BreadcrumbItems.FirstOrDefault(x =>
                    x.AssociatedPage?.Equals(viewModel) == true
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
                var itemsToClose = BreadcrumbItems.Skip(itemIndex).Reverse().ToArray();
                foreach (var itemToDispose in itemsToClose)
                {
                    if (!ClosePage(itemToDispose.AssociatedPage)) break;
                    BreadcrumbItems.Remove(itemToDispose);
                }
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

        #region Properties
        public MainViewModel? ViewModel { get => RootGrid?.DataContext as MainViewModel; }
        #endregion

        #region Handlers
        private void nvSample_Loaded(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(CourseBrowserPage));
        }
        private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is not BreadcrumbBarDataItem selectedItem) return;
            NavigateToPrevious(selectedItem);
        }
        private void OnPageNavigationRequestReceived(PageNavigationRequest request)
        {
            if (request == null) return;
            var viewModel = App.Current.Services.GetService(request.ViewModelType) as IPageViewModel;
            if (viewModel == null) return;
            switch (request.Action)
            {
                case NavigationRequestAction.GoNext: NavigateToNext(viewModel, request.DataParameter); break;
                case NavigationRequestAction.GoBack: NavigateToPrevious(viewModel, request.DataParameter); break;
                case NavigationRequestAction.ToRoot:
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
