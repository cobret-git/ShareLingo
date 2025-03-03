using CommunityToolkit.Mvvm.ComponentModel;
using NetForge.Core;
using NetForge.Core.EventArgs;
using System;
using System.Collections.Generic;

namespace ShareLingo.WinUI.ViewModel
{
    public abstract class PageViewModelBase : ObservableObject, IPageViewModel
    {
        #region Fields
        protected readonly IEventAggregator eventAggregator;
        protected readonly Dictionary<Type, int> subscribeTokens = new();
        private string header = string.Empty;
        #endregion

        #region Constructors
        public PageViewModelBase(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            subscribeTokens.Add(typeof(PageClosingEventArgs), eventAggregator.SubscribeAction<PageClosingEventArgs>(OnPageClosingBase));
            subscribeTokens.Add(typeof(PageClosedEventArgs), eventAggregator.SubscribeAction<PageClosedEventArgs>(OnPageClosedBase));
        }
        #endregion

        #region Properties
        public string Header { get => header; set { header = value; OnPropertyChanged(); } }
        public virtual IPageDataParameter? DataParameter { get; set; }
        public bool IsDisposed { get; }
        #endregion

        #region Methods
        public abstract void Dispose();
        #endregion

        #region Handlers
        private void OnPageClosingBase(PageClosingEventArgs e)
        {
            if (e.ViewModel.Equals(this)) OnPageClosing(e);
        }
        private void OnPageClosedBase(PageClosedEventArgs e)
        {
            if (e.ViewModel.Equals(this)) OnPageClosed(e);
        }
        protected abstract void OnPageClosing(PageClosingEventArgs e);
        protected abstract void OnPageClosed(PageClosedEventArgs e);
        #endregion
    }
}
