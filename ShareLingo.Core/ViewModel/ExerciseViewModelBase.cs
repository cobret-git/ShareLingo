using CommunityToolkit.Mvvm.ComponentModel;
using ShareLingo.Core.Model;

namespace ShareLingo.Core.ViewModel
{
    public abstract class ExerciseViewModelBase : ObservableObject, IDisposable
    {
        #region Constructors
        public ExerciseViewModelBase(ExerciseBase item)
        {
            Item = item;
        }
        #endregion

        #region Properties
        public ExerciseBase Item { get; }
        #endregion

        #region Methods
        public abstract void Dispose();
        #endregion
    }
}
