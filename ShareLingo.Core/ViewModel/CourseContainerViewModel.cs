using CommunityToolkit.Mvvm.ComponentModel;
using ShareLingo.Core.Model;

namespace ShareLingo.Core.ViewModel
{
    public class CourseContainerViewModel : ObservableObject, IDisposable
    {
        #region Constructors
        public CourseContainerViewModel(CourseContainer item)
        {
            Item = item;
        }
        #endregion

        #region Properties
        public CourseContainer Item { get; }
        public AttachedFileViewModel? Cover { get; set; }
        public AttachedFileViewModel? Description { get; set; }
        #endregion

        #region Methods
        public void Dispose()
        {
            if (Cover != null) Cover.Dispose();
            if (Description != null) Description.Dispose();
        }
        #endregion
    }
}
