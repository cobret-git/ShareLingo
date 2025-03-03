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
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Test
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await ShowConfirm("title_name", DialogButtons.YesNoCancel);
        }

        public async Task<DefaultDialogViewModel> ShowConfirm(string message, DialogButtons buttons)
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot.XamlRoot;
            dialog.Title = message;
            dialog.DefaultButton = ContentDialogButton.Primary;
            switch (buttons)
            {
                case DialogButtons.Ok:
                    dialog.PrimaryButtonText = "OK";
                    dialog.CloseButtonText = "Cancel";
                    dialog.IsSecondaryButtonEnabled = false;
                    break;
                case DialogButtons.OkCancel:
                    dialog.PrimaryButtonText = "OK";
                    dialog.CloseButtonText = "Cancel";
                    dialog.IsSecondaryButtonEnabled = false;
                    break;
                case DialogButtons.YesNo:
                    dialog.PrimaryButtonText = "Yes";
                    dialog.CloseButtonText = "No";
                    dialog.IsSecondaryButtonEnabled = false;
                    break;
                case DialogButtons.YesNoCancel:
                    dialog.PrimaryButtonText = "Yes";
                    dialog.SecondaryButtonText = "No";
                    dialog.CloseButtonText = "Cancel";
                    break;
                default:
                    break;
            }
            var result = await dialog.ShowAsync();
            return new DefaultDialogViewModel(string.Empty, message, buttons, DialogResult.Yes);
        }
    }
    public class DefaultDialogViewModel : IDialogViewModel
    {
        #region Constructors
        public DefaultDialogViewModel(string title, string message, DialogButtons buttons, DialogResult result)
        {
            Title = title;
            Message = message;
            Buttons = buttons;
            Result = result;
        }
        #endregion

        #region Properties
        public string Title { get; }
        public string Message { get; }
        public DialogButtons Buttons { get; }
        public DialogResult Result { get; set; }
        #endregion
    }
    public enum DialogButtons { Ok, OkCancel, YesNo, YesNoCancel }
    public enum DialogResult { None, Ok, Yes, No, Cancel }
    public interface IDialogViewModel
    {
        string Title { get; }
        string Message { get; }
        DialogButtons Buttons { get; }
        DialogResult Result { get; }
    }
}
