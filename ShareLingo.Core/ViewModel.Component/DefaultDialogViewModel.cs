namespace ShareLingo.Core.ViewModel.Component
{
    public class DefaultDialogViewModel : IDialogViewModel
    {
        #region Constructors
        public DefaultDialogViewModel(string title, string message)
        {
            Title = title;
            Message = message;
        }
        #endregion

        #region Properties
        public string Title { get; }
        public string Message { get; }
        public DialogButtons Buttons { get; }
        public DialogResult Result { get; set; }
        #endregion
    }
}
