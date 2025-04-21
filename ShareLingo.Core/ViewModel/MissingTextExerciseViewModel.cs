using ShareLingo.Core.Model;
using System.Collections.ObjectModel;

namespace ShareLingo.Core.ViewModel
{
    public class MissingTextExerciseViewModel : ExerciseViewModelBase
    {
        #region Constructors
        public MissingTextExerciseViewModel(ExerciseBase item, ModuleItemViewModel moduleItem) : base(item, moduleItem)
        {
        }
        #endregion

        #region Properties
        public ObservableCollection<MissingTextExerciseItemViewModel> Items { get; set; } = new();
        #endregion

        #region Methods
        public override void Dispose()
        {
        }
        public override IEnumerable<ExerciseSubItemViewModelBase> GetSubItems()
        {
            return Items;
        }
        #endregion
    }
}
