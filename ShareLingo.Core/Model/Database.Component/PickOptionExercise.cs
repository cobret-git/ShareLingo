namespace ShareLingo.Core.Model.Database.Component
{
    public class PickOptionExercise : ExerciseContentBase
    {
        #region Properties
        public string Description { get; set; } = string.Empty;
        public PickOptionSubItem[] Options { get; set; } = Array.Empty<PickOptionSubItem>();
        #endregion
    }
    public struct PickOptionSubItem
    {
        #region Properties
        public string Value { get; set; }
        public bool IsCorrect { get; set; }
        #endregion
    }
}
