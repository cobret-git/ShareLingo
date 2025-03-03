using LiteDB;

namespace ShareLingo.WinUI.Model.Database.Component
{
    public class ResultContainer
    {
        #region Consts
        public const string COLLECTION_NAME = "Results";
        #endregion

        #region Properties
        [BsonId] public int Id { get; set; }
        [BsonRef(ExerciseContainer.COLLECTION_NAME)] public ExerciseContainer Exercise { get; set; } = null!;
        public byte Score { get; set; }
        public DateTime LastCompleteOn { get; set; }
        public ResultAttempt[] Attempts { get; set; } = Array.Empty<ResultAttempt>();
        #endregion
    }
    public class ResultAttempt
    {
        #region Properties
        public DateTime CompleteOn { get; set; }
        public byte Score { get; set; }
        public int[] ErrorIds { get; set; } = Array.Empty<int>();
        #endregion
    }
}
