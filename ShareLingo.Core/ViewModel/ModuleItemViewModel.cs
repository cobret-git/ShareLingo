using CommunityToolkit.Mvvm.ComponentModel;
using ShareLingo.Core.Model;

namespace ShareLingo.Core.ViewModel
{
    public class ModuleItemViewModel : ObservableObject, IDisposable
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
        public void Dispose()
        {
            if (Cover != null) Cover.Dispose();
            if (Theory != null) Theory.Dispose();
        }
        #endregion
    }
}
