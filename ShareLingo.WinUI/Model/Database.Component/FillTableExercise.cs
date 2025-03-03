namespace ShareLingo.WinUI.Model.Database.Component
{
    public class FillTableExercise : ExerciseContentBase
    {
        #region Properties
        public string Description { get; set; } = string.Empty;
        #endregion
    }
    public struct FillTableSubItem
    {
        #region Properties
        public int Id { get; set; }
        public string LeftItem { get; set; }
        #endregion
    }
}
