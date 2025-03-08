using CommunityToolkit.Mvvm.ComponentModel;
using NetForge.Core;

namespace ShareLingo.WinUI.ViewModel
{
    public partial class ModuleItemViewModel : ObservableObject, IPageDataParameter
    {
        #region Fields
        #endregion

        #region Properties
        [ObservableProperty] public partial string Name { get; set; } = string.Empty;
        #endregion
    }
}
