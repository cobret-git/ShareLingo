using CommunityToolkit.Mvvm.ComponentModel;
using ShareLingo.Core.Model;
using System.Collections.ObjectModel;

namespace ShareLingo.Core.ViewModel
{
    public abstract class ExerciseSubItemViewModelBase : ObservableObject, IDisposable
    {
        #region Constructors
        protected ExerciseSubItemViewModelBase(ExerciseSubItemBase item, ExerciseViewModelBase parent)
        {
            Item = item;
            Parent = parent;
        }
        #endregion

        #region Properties
        public ExerciseViewModelBase Parent { get; }
        public ExerciseSubItemBase Item { get; }
        public ObservableCollection<AttachedTagViewModel> Tags { get; set; } = new();
        public bool Correct { get; set; } = false;
        public int AttemptsCount { get; set; }
        #endregion

        #region Methods
        public abstract void Dispose();
        #endregion
    }
}
