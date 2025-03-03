using CommunityToolkit.Mvvm.ComponentModel;

namespace ShareLingo.WinUI.ViewModel
{
    public partial class ModuleItemViewModel : ObservableObject
    {
        #region Fields
        #endregion

        #region Properties
        [ObservableProperty] public partial string Name { get; set; } = string.Empty;
        #endregion
    }
}
