namespace ShareLingo.Core.ViewModel.Component
{
    public class PromptDialogViewModel : IDialogViewModel
    {

        #region Constructors
        public PromptDialogViewModel(string title, string message, IPromptValidator? validator)
        {
            Title = title;
            Message = message;
            Validator = validator;
        }
        #endregion

        #region Properties
        public string Title { get; }
        public string Message { get; }
        public IPromptValidator? Validator { get; }
        public string Prompt { get; set; } = null!;
        public DialogButtons Buttons { get; } = DialogButtons.Ok;
        public DialogResult Result { get; set; }
        #endregion
    }
    public interface IPromptValidator
    {
        bool IsValid(string? prompt);
    }
}
