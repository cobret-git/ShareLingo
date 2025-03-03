namespace ShareLingo.WinUI.Model.Database.Component
{
    public class TypeMissingWordExercise : ExerciseContentBase
    {
        #region Properties
        public string Description { get; set; } = string.Empty;
        public TypeMissingWordSubItem[] Items { get; set; } = Array.Empty<TypeMissingWordSubItem>();
        #endregion
    }
    public struct TypeMissingWordSubItem
    {
        #region Properties
        public int Id { get; set; }
        public string Content { get; set; }
        public string MissingContent { get; set; }
        #endregion
    }
}
