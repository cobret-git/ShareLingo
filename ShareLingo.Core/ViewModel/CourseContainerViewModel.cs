using CommunityToolkit.Mvvm.ComponentModel;
using NetForge.Core;
using ShareLingo.Core.Model;

namespace ShareLingo.Core.ViewModel
{
    public class CourseContainerViewModel : ObservableObject, IDisposable, IViewModelDataParameter, ICloneable<CourseContainerViewModel>, IMergable<CourseContainerViewModel>
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
        public CourseContainerViewModel Clone()
        {
            return new CourseContainerViewModel(Item.Clone())
            {
                Cover = Cover?.Clone(),
                Description = Description?.Clone()
            };
        }
        public void Merge(CourseContainerViewModel other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            Item.Merge(other.Item);
            if (other.Cover != null) Cover?.Merge(other.Cover);
            if (other.Description != null) Description?.Merge(other.Description);
        }
        public void Dispose()
        {
            if (Cover != null) Cover.Dispose();
            if (Description != null) Description.Dispose();
        }
        #endregion
    }
}
