using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareLingo.WinUI.ViewModel.Component;
using System.Collections.ObjectModel;
using msg = ShareLingo.Core.Resources.Messages;
using fpr = ShareLingo.Core.Resources.FileProperties;
using System.Globalization;
using ShareLingo.WinUI.Services;
using System;

namespace ShareLingo.WinUI.ViewModel
{
    public partial class CourseInspectorViewModel : ObservableObject
    {
        #region Fields
        private readonly ILoggerManager logger;
        private readonly IContentManager contentManager;
        private readonly IDataManager dataManager;
        private CourseContainerViewModel initialCourse = null!;
        [ObservableProperty, 
            NotifyCanExecuteChangedFor(nameof(BeginEditCommand), 
            nameof(SaveChangesCommand), nameof(CancelChangesCommand))] 
        private bool editing;
        #endregion

        #region Constructors
        public CourseInspectorViewModel()
        {
            CultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
        }
        #endregion

        #region Properties
        public CourseContainerViewModel Course { get; private set; } = null!;
        public ObservableCollection<ModuleItemViewModel> Modules { get; } = new();
        public CultureInfo[] CultureInfos { get; }
        #endregion

        #region Methods
        public void SetCourse(CourseContainerViewModel course)
        {
            this.Course = (CourseContainerViewModel)course.Clone();
            this.initialCourse = course;
        }
        [RelayCommand(CanExecute = nameof(Editing))] private async Task ChangeCourseCover()
        {
            try
            {
                var openDlg = await contentManager.OpenFile(fpr.image_dlgFilter);
                if (!openDlg.FileSelected) return;
                var data = dataManager.Media.ImportCourseImage(openDlg.FileName);
                Course.PictureCoverPath = data.RelativePath;
                Course.PictureCoverAbsolutePath = data.AbsolutePath;
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        [RelayCommand(CanExecute = nameof(CanBeginEdit))] private void BeginEdit()
        {
            try
            {
                Editing = true;
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        [RelayCommand(CanExecute = nameof(CanSaveChanges))] private async Task SaveChanges()
        {
            try
            {
                //var validator = dataManager.GetCourseNameValidator();
                //if (!validator.IsValid(Course.Name))
                //{
                //    await contentManager.ShowError(msg.courseInspector_enteredCourseNameWrong);
                //    return;
                //}
                //var nameOccupied = dataManager.GetContaienrs(0, int.MaxValue).Where(x => x.Name == Course.Name && x.Id != Course.Id).Any();
                //if (nameOccupied) 
                //{
                //    await contentManager.ShowError(msg.courseInspector_enteredCourseNameAlreadyInUse);
                //    return;
                //}
                //if (string.IsNullOrWhiteSpace(Course.NativeLanguageCode) 
                //    || !CultureInfos.Select(x => CultureInfo.CreateSpecificCulture(x.Name).Name).Contains(Course.NativeLanguageCode))
                //{
                //    await contentManager.ShowError(msg.courseInspector_enteredNativeLanguageWrong);
                //    return;
                //}
                //if (string.IsNullOrWhiteSpace(Course.ForeignLanguageCode)
                //    || !CultureInfos.Select(x => CultureInfo.CreateSpecificCulture(x.Name).Name).Contains(Course.ForeignLanguageCode))
                //{
                //    await contentManager.ShowError(msg.courseInspector_enteredForeignLanguageWrong);
                //    return;
                //}
                //dataManager.SaveCourse(Course);
                //this.initialCourse.Merge(Course);
                //Editing = false;
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        [RelayCommand(CanExecute = nameof(CanCancelChanges))] private void CancelChanges()
        {
            try
            {
                Course.Merge(initialCourse);
                Editing = false;
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        [RelayCommand(CanExecute = nameof(CanCreateModule))] private async Task CreateModule()
        {
            try
            {
                //var validator = dataManager.GetCourseNameValidator();
                //var promptVm = await contentManager.ShowPrompt(msg.courseInspector_enterModuleNamePrompt, validator);
                //if (promptVm.Result != DialogResult.Ok) return;
                //if (!validator.IsValid(promptVm.Prompt)) return;
                //if (Modules.Any(x => x.Name == promptVm.Prompt))
                //    await contentManager.ShowError(msg.courseInspector_enteredModuleNameAlreadyInUse);
                //else
                //{
                //    var module = dataManager.CreateModule(promptVm.Prompt);
                //    Modules.Add(module);
                //    contentManager.InspectModule(module);
                //}
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        [RelayCommand(CanExecute = nameof(CanInspectModule))] private void InspectModule(ModuleItemViewModel? item)
        {
            try
            {
                if (item == null) return;
                contentManager.InspectModule(item);
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        [RelayCommand(CanExecute = nameof(CanDeleteModule))] private void DeleteModule(ModuleItemViewModel? item)
        {
            try
            {
                if (item == null) return;
                dataManager.DeleteModule(item);
                Modules.Remove(item);
            }
            catch (Exception ex) { logger.Debug(ex); }
        }
        #endregion

        #region Helpers
        private bool CanBeginEdit()
        {
            return !Editing;
        }
        private bool CanSaveChanges()
        {
            return !Editing;
        }
        private bool CanCancelChanges()
        {
            return Editing;
        }
        private bool CanDeleteModule() => !Editing;
        private bool CanInspectModule() => !Editing;
        private bool CanCreateModule() => !Editing;
        #endregion
    }
}
