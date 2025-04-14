using CommunityToolkit.Mvvm.ComponentModel;
using ShareLingo.Core.Model;
using System.Collections.ObjectModel;

namespace ShareLingo.Core.ViewModel
{
    public abstract class ExerciseSubItemViewModelBase : ObservableObject, IDisposable
    {
        #region Constructors
        protected ExerciseSubItemViewModelBase(ExerciseSubItemBase item)
        {
            Item = item;
        }
        #endregion

        #region Properties
        public ExerciseSubItemBase Item { get; }
        public ObservableCollection<TagRelation> Tags { get; init; } = new();
        #endregion

        #region Methods
        public abstract void Dispose();
        #endregion
    }
}
