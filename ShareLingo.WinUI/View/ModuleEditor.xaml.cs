using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using ShareLingo.Core.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShareLingo.WinUI.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModuleEditor : Page
    {
        public ModuleEditor()
        {
            this.DataContext = App.Current.Services.GetService<ModuleEditorViewModel>();
            this.InitializeComponent();
        }

        private Expander _currentlyExpanded = null;
        private void Expander_Expanding(Expander sender, ExpanderExpandingEventArgs args)
        {
            Expander expandingExpander = sender as Expander;

            if (_currentlyExpanded != null && _currentlyExpanded != expandingExpander)
            {
                _currentlyExpanded.IsExpanded = false;
            }

            _currentlyExpanded = expandingExpander;
        }
    }
}
