using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using NetForge.Core;
using ShareLingo.WinUI.View;
using ShareLingo.WinUI.ViewModel;
using System;
using System.Threading.Tasks;

namespace ShareLingo.WinUI.Components
{
    public class MessageDialogHelper
    {
        #region Fields
        private ContentDialog? promptDialog;
        #endregion

        #region Properties
        public XamlRoot XamlRoot { get; set; } = null!;
        #endregion

        #region Methods
        public async Task<DefaultDialogViewModel> ShowWarn(string message)
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = XamlRoot;
            dialog.Title = message;
            dialog.DefaultButton = ContentDialogButton.Primary;
            SetDialogButtons(dialog, DialogButtons.Ok);
            var result = await dialog.ShowAsync();
            return new DefaultDialogViewModel(message, DialogButtons.Ok, ConvertToResult(result, DialogButtons.Ok));
        }
        public async Task<DefaultDialogViewModel> ShowError(string message)
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = XamlRoot;
            dialog.Title = message;
            dialog.DefaultButton = ContentDialogButton.Primary;
            SetDialogButtons(dialog, DialogButtons.Ok);
            var result = await dialog.ShowAsync();
            return new DefaultDialogViewModel(message, DialogButtons.Ok, ConvertToResult(result, DialogButtons.Ok));
        }
        public async Task<DefaultDialogViewModel> ShowInfo(string message)
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = XamlRoot;
            dialog.Title = message;
            dialog.DefaultButton = ContentDialogButton.Primary;
            SetDialogButtons(dialog, DialogButtons.Ok);
            var result = await dialog.ShowAsync();
            return new DefaultDialogViewModel(message, DialogButtons.Ok, ConvertToResult(result, DialogButtons.Ok));
        }
        public async Task<DefaultDialogViewModel> ShowConfirm(string message, DialogButtons buttons)
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = XamlRoot;
            dialog.Title = message;
            dialog.DefaultButton = ContentDialogButton.Primary;
            SetDialogButtons(dialog, buttons);
            var result = await dialog.ShowAsync();
            return new DefaultDialogViewModel(message, buttons, ConvertToResult(result, buttons));
        }
        public async Task<PromptDialogViewModel> ShowPrompt(string message, IAttachedTextValidator? validator)
        {
            PromptDialogViewModel? viewModel = null;
            try
            {
                var dialog = new ContentDialog();
                this.promptDialog = dialog;
                viewModel = new PromptDialogViewModel();
                viewModel.ValidationResultChanged += ViewModel_ValidationResultChanged;
                dialog.XamlRoot = XamlRoot;
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = new PromptDialogControl() { DataContext = viewModel };
                SetDialogButtons(dialog, DialogButtons.OkCancel);
                var result = await dialog.ShowAsync();
                return viewModel;
            }
            finally
            {
                if (viewModel != null) viewModel.ValidationResultChanged += ViewModel_ValidationResultChanged;
            }
            
        }
        #endregion

        #region Handlers
        private void ViewModel_ValidationResultChanged(object? sender, bool e)
        {
            if (promptDialog != null)
                promptDialog.IsPrimaryButtonEnabled = e;
        }
        #endregion

        #region Helpers
        private void SetDialogButtons(ContentDialog dialog, DialogButtons buttons)
        {
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
        }
        private DialogResult ConvertToResult(ContentDialogResult result, DialogButtons buttons)
        {
            switch (buttons)
            {
                case DialogButtons.Ok: return result == ContentDialogResult.Primary ? DialogResult.Ok : DialogResult.None;
                case DialogButtons.OkCancel: return result == ContentDialogResult.Primary ? DialogResult.Ok : DialogResult.None;
                case DialogButtons.YesNo: return result == ContentDialogResult.Primary ? DialogResult.Yes : DialogResult.No;
                case DialogButtons.YesNoCancel:
                    if (result == ContentDialogResult.Primary) return DialogResult.Yes;
                    else if (result == ContentDialogResult.Secondary) return DialogResult.No;
                    else return DialogResult.Cancel;
                default: return DialogResult.None;
            }
        }
        #endregion
    }
}
