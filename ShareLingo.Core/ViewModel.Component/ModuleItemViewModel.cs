using CommunityToolkit.Mvvm.ComponentModel;

namespace ShareLingo.Core.ViewModel.Component
{
    public partial class ModuleItemViewModel : ObservableObject
    {
        #region Fields
        [ObservableProperty] private string name;
        #endregion
    }
}
