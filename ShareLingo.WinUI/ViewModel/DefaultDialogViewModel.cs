namespace ShareLingo.WinUI.ViewModel
{
    public class DefaultDialogViewModel : IDialogViewModel
    {
        #region Constructors
        public DefaultDialogViewModel(string message, DialogButtons buttons, DialogResult result)
        {
            Message = message;
            Buttons = buttons;
            Result = result;
        }
        #endregion

        #region Properties
        public string Message { get; }
        public DialogButtons Buttons { get; }
        public DialogResult Result { get; set; }
        #endregion
    }
}
