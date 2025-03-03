using NetForge.Core;

namespace ShareLingo.WinUI.ViewModel.Component
{
    public class BreadcrumbBarDataItem
    {
        #region Constructors
        public BreadcrumbBarDataItem(IPageViewModel page)
        {
            AssociatedPage = page;
        }
        #endregion

        #region Properties
        public string Header { get => AssociatedPage?.Header; }
        public IPageViewModel AssociatedPage { get; }
        #endregion
    }
}
