using CommunityToolkit.Mvvm.ComponentModel;
using NetForge.Core;

namespace ShareLingo.Core.ViewModel
{
    public class DialogViewModelBase : ObservableObject, IDialogViewModel
    {
        #region Fields

        #endregion

        #region Properties
        public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsDisposed { get; protected set; }
        public string? PrimaryButtonText => throw new NotImplementedException();
        public bool PrimaryButtonEnabled => throw new NotImplementedException();
        public string? SecondaryButtonText => throw new NotImplementedException();
        public bool SecondaryButtonEnabled => throw new NotImplementedException();
        public string? CloseButtonText => throw new NotImplementedException();
        public IViewModelDataParameter? DataParameter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion



        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
