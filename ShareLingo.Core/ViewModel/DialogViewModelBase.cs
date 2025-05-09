using CommunityToolkit.Mvvm.ComponentModel;
using NetForge.Core;

namespace ShareLingo.Core.ViewModel
{
    public abstract class DialogViewModelBase : ObservableObject, IDialogViewModel
    {
        #region Fields
        private string title = string.Empty;
        private bool primaryButtonEnabled = true;
        private bool secondaryButtonEnabled = true;
        #endregion

        #region Properties
        public string Title { get => title; set { title = value; OnPropertyChanged(); } }
        public bool IsDisposed { get; protected set; }
        public string? PrimaryButtonText { get; protected init; }
        public bool PrimaryButtonEnabled { get => primaryButtonEnabled; protected set { primaryButtonEnabled = value; OnPropertyChanged(); } }
        public string? SecondaryButtonText { get; protected init; }
        public bool SecondaryButtonEnabled { get => secondaryButtonEnabled; protected set { secondaryButtonEnabled = value; OnPropertyChanged(); } }
        public string? CloseButtonText { get; protected init; }
        public virtual IViewModelDataParameter? DataParameter { get; set; }
        #endregion

        #region Methods
        public abstract void Dispose();
        #endregion
    }
}
