using ShareLingo.Core.Model;
using System.Collections.ObjectModel;

namespace ShareLingo.Core.ViewModel
{
    public class MissingTextExerciseViewModel : ExerciseViewModelBase
    {
        #region Constructors
        public MissingTextExerciseViewModel(MissingTextExercise item) : base(item)
        {
        }
        #endregion

        #region Properties
        public ObservableCollection<MissingTestExerciseItemViewModel> Items { get; init; } = new();
        #endregion

        #region Methods
        public override void Dispose()
        {
        }
        #endregion
    }
}
