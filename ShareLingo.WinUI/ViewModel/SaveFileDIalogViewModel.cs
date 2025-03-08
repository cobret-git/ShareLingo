namespace ShareLingo.WinUI.ViewModel
{
    public class SaveFileDialogViewModel
    {
        #region Constructors
        public SaveFileDialogViewModel(string? fileName, bool fileSelected)
        {
            FileName = fileName!;
            FileSelected = fileSelected;
        }
        #endregion

        #region Properties
        public string FileName { get; }
        public bool FileSelected { get; }
        #endregion
    }
}
