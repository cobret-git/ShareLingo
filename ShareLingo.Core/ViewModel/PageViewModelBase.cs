using CommunityToolkit.Mvvm.ComponentModel;
using NetForge.Core;

namespace ShareLingo.Core.ViewModel
{
    public abstract class PageViewModelBase : ObservableObject, IPageViewModel
    {
        #region Fields
        protected readonly IEventAggregator eventAggregator;
        private string header = string.Empty;
        private readonly Dictionary<Type, int> _subscribeTokens = new();
        #endregion

        #region Constructors
        protected PageViewModelBase(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            _subscribeTokens.Add(typeof(PageClosingEventArgs),
                eventAggregator.SubscribeAction<PageClosingEventArgs>(_OnPageClosing));
            _subscribeTokens.Add(typeof(PageClosedEventArgs),
                eventAggregator.SubscribeAction<PageClosedEventArgs>(_OnPageClosed));
        }
        #endregion

        #region Properties
        public string Header { get => header; set { header = value; OnPropertyChanged(); } }
        public bool IsDisposed { get; protected set; }
        public virtual IViewModelDataParameter? DataParameter { get; set; }
        #endregion

        #region Methods
        public abstract void Dispose();
        #endregion

        #region Handlers
        protected virtual void OnPageClosing(PageClosingEventArgs e)
        {
        }
        protected virtual void OnPageClosed(PageClosedEventArgs e)
        {
        }
        private void _OnPageClosing(PageClosingEventArgs e)
        {
            if (e.ViewModel?.Equals(this) == true) OnPageClosing(e);
        }
        private void _OnPageClosed(PageClosedEventArgs e)
        {
            if (e.ViewModel?.Equals(this) == true) OnPageClosed(e);
            foreach(var key in  _subscribeTokens.Keys)
                eventAggregator.UnsubscribeAction(key, _subscribeTokens[key]);
        }
        #endregion
    }
}
