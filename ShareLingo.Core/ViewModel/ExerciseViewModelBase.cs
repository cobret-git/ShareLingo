using CommunityToolkit.Mvvm.ComponentModel;
using ShareLingo.Core.Model;

namespace ShareLingo.Core.ViewModel
{
    public abstract class ExerciseViewModelBase : ObservableObject, IDisposable
    {
        #region Constructors
        public ExerciseViewModelBase(ExerciseBase item, ModuleItemViewModel moduleItem)
        {
            Item = item;
            ModuleItem = moduleItem;
        }
        #endregion

        #region Properties
        public ModuleItemViewModel ModuleItem { get; }
        public ExerciseBase Item { get; }
        #endregion

        #region Methods
        public abstract void Dispose();
        public abstract IEnumerable<ExerciseSubItemViewModelBase> GetSubItems();
        #endregion
    }
}
