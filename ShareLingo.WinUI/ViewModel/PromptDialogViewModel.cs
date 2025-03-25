using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetForge.Core;
using System;

namespace ShareLingo.WinUI.ViewModel
{
    public partial class PromptDialogViewModel : ObservableObject
    {
        #region Fields
        private string errorMessage = string.Empty;
        private bool isValid = true;
        private string prompt = string.Empty;
        #endregion

        #region Constructors
        public PromptDialogViewModel()
        {
        }
        #endregion

        #region Events
        public event EventHandler<bool>? ValidationResultChanged;
        #endregion

        #region Properties
        public string Message { get; init; } = string.Empty;
        public string ErrorMessage { get => errorMessage; private set { errorMessage = value; OnPropertyChanged(); } }
        public bool IsValid { get => isValid; private set { isValid = value; OnPropertyChanged(); } }
        public string Prompt { get => prompt; set { prompt = value; OnPromptChanged(value); } }
        public DialogResult Result { get; set; }
        public IAttachedTextValidator? Validator { get; init; }
        #endregion

        #region Methods
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
            ValidationResultChanged?.Invoke(this, IsValid);
        }
        #endregion

    }
}
