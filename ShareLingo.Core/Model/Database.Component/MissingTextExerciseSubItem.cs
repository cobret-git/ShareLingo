namespace ShareLingo.Core.Model
{
    public class MissingTextExerciseSubItem : ExerciseSubItemBase
    {
        #region Fields
        private string text = string.Empty;
        #endregion

        #region Properties
        public string Text { get => text; set { text = value; OnPropertyChanged(); } }
        #endregion
    }
}
