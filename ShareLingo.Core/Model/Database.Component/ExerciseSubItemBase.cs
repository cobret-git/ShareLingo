using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;

namespace ShareLingo.Core.Model
{
    public abstract class ExerciseSubItemBase : ObservableObject
    {
        #region Consts
        public const string COLLECTION_NAME = "ExerciseSubItems";
        #endregion

        #region Fields
        private DateTime lastAttempt = default;
        private int attemptCount = 0;
        private double score;
        #endregion

        #region Properties
        [BsonId] public int Id { get; set; }
        [BsonRef(ExerciseBase.COLLECTION_NAME)] public ExerciseBase Exercise { get; set; } = null!;
        public DateTime LastAttempt { get => lastAttempt; set { lastAttempt = value; OnPropertyChanged(); } }
        public int AttemptCount { get => attemptCount; set { attemptCount = value; OnPropertyChanged(); } }
        public double Score { get => score; set { score = value; OnPropertyChanged(); } }
        #endregion

        #region Methods
        public double GetPriotity()
        {
            var daysSince = (DateTime.Now - LastAttempt).TotalDays;
            double decay = Math.Exp(-0.1 * daysSince); // You can tweak lambda
            return (1.0 - Score) * decay;
        }
        #endregion
    }
}
