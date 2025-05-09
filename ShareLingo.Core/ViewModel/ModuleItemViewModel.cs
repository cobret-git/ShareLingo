using CommunityToolkit.Mvvm.ComponentModel;
using NetForge.Core;
using ShareLingo.Core.Model;

namespace ShareLingo.Core.ViewModel
{
    public class ModuleItemViewModel : ObservableObject, IDisposable, IViewModelDataParameter, ICloneable<ModuleItemViewModel>, IMergable<ModuleItemViewModel>
    {
        #region Constructors
        public ModuleItemViewModel(ModuleItem item, CourseContainerViewModel course)
        {
            Item = item;
            Course = course;
        }
        #endregion

        #region Properties
        public ModuleItem Item { get; }
        public CourseContainerViewModel Course { get; }
        public AttachedFileViewModel? Cover { get; set; }
        public AttachedFileViewModel? Theory { get; set; }
        #endregion

        #region Methods
        public ModuleItemViewModel Clone()
        {
            return new ModuleItemViewModel(Item.Clone(), Course)
            {
                Cover = Cover?.Clone(),
                Theory = Theory?.Clone()
            };
        }
        public void Merge(ModuleItemViewModel other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            Item.Merge(other.Item);
            if (other.Cover != null) Cover?.Merge(other.Cover);
            if (other.Theory != null) Theory?.Merge(other.Theory);
        }
        public void Dispose()
        {
            if (Cover != null) Cover.Dispose();
            if (Theory != null) Theory.Dispose();
        }
        #endregion
    }
}
