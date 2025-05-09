using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using NetForge.Core;
using ShareLingo.Core.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShareLingo.WinUI.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CourseBrowser : Page
    {
        public CourseBrowser()
        {
            this.DataContext = App.Current.Services.GetService<CourseBrowserViewModel>();
            this.InitializeComponent();
        }
        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    base.OnNavigatedTo(e);
        //    if (DataContext is IPageViewModel pageViewModel
        //        && e.Parameter is IViewModelDataParameter dataParameter) pageViewModel.DataParameter = dataParameter;
        //}
    }
}
