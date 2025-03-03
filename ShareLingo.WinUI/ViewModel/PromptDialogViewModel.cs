using CommunityToolkit.Mvvm.ComponentModel;
using NetForge.Core;

namespace ShareLingo.WinUI.ViewModel
{
    public partial class PromptDialogViewModel : ObservableObject, IDialogViewModel
    {
        #region Fields
        private string errorMessage = string.Empty;
        private bool isValid = true;
        private string prompt = string.Empty;
        #endregion

        #region Properties
        public string Message { get; init; } = string.Empty;
        public string ErrorMessage { get => errorMessage; private set { errorMessage = value; OnPropertyChanged(); } }
        public bool IsValid { get => isValid; private set { isValid = value; OnPropertyChanged(); } }
        public string Prompt { get => prompt; set { prompt = value; OnPromptChanged(value); } }
        public DialogButtons Buttons { get; init; }
        public DialogResult Result { get; init; }
        public IAttachedTextValidator? Validator { get; }
        #endregion

        #region Handlers
        private void OnPromptChanged(string value)
        {
            if (Validator != null)
            {
                IsValid = Validator.IsValid(value, out var _errorMessage);
                ErrorMessage = _errorMessage;
            }
            OnPropertyChanged(nameof(Prompt));
        }
        #endregion

    }
}
