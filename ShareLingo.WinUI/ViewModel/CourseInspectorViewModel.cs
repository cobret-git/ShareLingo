using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareLingo.WinUI.ViewModel.Component;
using System.Collections.ObjectModel;
using MSG = ShareLingo.WinUI.Resources.Strings.Messages;
using FPR = ShareLingo.WinUI.Resources.Strings.FileProperties;
using System.Globalization;
using ShareLingo.WinUI.Services;
using System;
using System.Threading.Tasks;
using NetForge.Core;
using ShareLingo.WinUI.Extensions;
using NetForge.Core.EventArgs;
using System.Linq;

namespace ShareLingo.WinUI.ViewModel
{
    public partial class CourseInspectorViewModel : PageViewModelBase
    {
        #region Fields
        private readonly IContentManager contentManager;
        private readonly IDataManager dataManager;
        private CourseContainerViewModel initialCourse = null!;
        #endregion

        #region Constructors
        public CourseInspectorViewModel(IEventAggregator eventAggregator, IContentManager contentManager, IDataManager dataManager)
            : base(eventAggregator)
        {
            this.contentManager = contentManager;
            this.dataManager = dataManager;

            CultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
        }
        #endregion

        #region Properties
        public CourseContainerViewModel Course { get; private set; } = null!;
        public ObservableCollection<ModuleItemViewModel> Modules { get; } = new();
        [ObservableProperty] public partial bool Editing { get; set; }
        public CultureInfo[] CultureInfos { get; }
        public override IViewModelDataParameter? DataParameter { get => initialCourse; set => SetCourse(value as CourseContainerViewModel); }
        #endregion

        #region Methods
        public override void Dispose()
        {
            IsDisposed = true;
        }
        
        
        [RelayCommand(CanExecute = nameof(CanBeginEdit))] private void BeginEdit()
        {
            try
            {
                Editing = true;
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand(CanExecute = nameof(CanSaveChanges))] private async Task SaveChanges()
        {
            try
            {
                var validator = dataManager.GetCourseNameValidator();
                if (!validator.IsValid(Course.Name, out _))
                {
                    await contentManager.ShowError(MSG.courseInspector_enteredCourseNameWrong);
                    return;
                }
                var nameOccupied = dataManager.GetContaienrs(0, int.MaxValue).Where(x => x.Name == Course.Name && x.Id != Course.Id).Any();
                if (nameOccupied)
                {
                    await contentManager.ShowError(MSG.courseInspector_enteredCourseNameAlreadyInUse);
                    return;
                }
                if (string.IsNullOrWhiteSpace(Course.NativeLanguageCode)
                    || !CultureInfos.Select(x => CultureInfo.CreateSpecificCulture(x.Name).Name).Contains(Course.NativeLanguageCode))
                {
                    await contentManager.ShowError(MSG.courseInspector_enteredNativeLanguageWrong);
                    return;
                }
                if (string.IsNullOrWhiteSpace(Course.ForeignLanguageCode)
                    || !CultureInfos.Select(x => CultureInfo.CreateSpecificCulture(x.Name).Name).Contains(Course.ForeignLanguageCode))
                {
                    await contentManager.ShowError(MSG.courseInspector_enteredForeignLanguageWrong);
                    return;
                }
                dataManager.SaveCourse(Course);
                this.initialCourse.Merge(Course);
                Editing = false;
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand(CanExecute = nameof(CanCancelChanges))] private void CancelChanges()
        {
            try
            {
                Course.Merge(initialCourse);
                Editing = false;
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand(CanExecute = nameof(CanCreateModule))] private async Task CreateModule()
        {
            try
            {
                var validator = dataManager.GetCourseNameValidator();
                var promptVm = await contentManager.ShowPrompt(MSG.courseInspector_enterModuleNamePrompt, validator);
                if (promptVm.Result != DialogResult.Ok) return;
                if (!validator.IsValid(promptVm.Prompt, out _)) return;
                if (Modules.Any(x => x.Name == promptVm.Prompt))
                    await contentManager.ShowError(MSG.courseInspector_enteredModuleNameAlreadyInUse);
                else
                {
                    var module = dataManager.CreateModule(promptVm.Prompt);
                    Modules.Add(module);
                    var request = PageSource.ModuleInspector.ToRequest(NavigationRequestAction.GoNext, module);
                    eventAggregator.Publish(request);
                }
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand(CanExecute = nameof(CanInspectModule))] private void InspectModule(ModuleItemViewModel? item)
        {
            try
            {
                if (item == null) return;
                var request = PageSource.ModuleInspector.ToRequest(NavigationRequestAction.GoNext, item);
                eventAggregator.Publish(request);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand(CanExecute = nameof(CanDeleteModule))] private void DeleteModule(ModuleItemViewModel? item)
        {
            try
            {
                if (item == null) return;
                dataManager.DeleteModule(item);
                Modules.Remove(item);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        #endregion

        #region Handlers
        partial void OnEditingChanged(bool value)
        {
            NotifyCanExecute();
        }
        protected override async void OnPageClosing(PageClosingEventArgs e)
        {
            try
            {
                if (!Editing) return;
                var reply = await contentManager.ShowConfirm(MSG.courseInspector_saveChangesConfirm, DialogButtons.YesNoCancel);
                switch (reply.Result)
                {
                    case DialogResult.None: e.IsCanceled = true; break;
                    case DialogResult.Yes: await SaveChanges(); e.IsCanceled = Editing; break;
                    case DialogResult.No: e.IsCanceled = false; break;
                    case DialogResult.Cancel: e.IsCanceled = true; break;
                    default: throw new Exception("Unexpected reply.");
                }
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }            
        }
        protected override void OnPageClosed(PageClosedEventArgs e)
        {
            Dispose();
        }
        #endregion

        #region Helpers
        private void SetCourse(CourseContainerViewModel? course)
        {
            if (course == null) return;
            this.Course = (CourseContainerViewModel)course.Clone();
            this.initialCourse = course;
            Header = Course.Name;
        }
        private void NotifyCanExecute()
        {
            eventAggregator.InvokeActionOnUIThread(() =>
            {
                BeginEditCommand.NotifyCanExecuteChanged();
                SaveChangesCommand.NotifyCanExecuteChanged();
                CancelChangesCommand.NotifyCanExecuteChanged();
                DeleteModuleCommand.NotifyCanExecuteChanged();
                InspectModuleCommand.NotifyCanExecuteChanged();
                CreateModuleCommand.NotifyCanExecuteChanged();
            });
        }
        private bool CanBeginEdit() => !Editing;
        private bool CanSaveChanges() => !Editing;
        private bool CanCancelChanges() => Editing;
        private bool CanDeleteModule() => !Editing;
        private bool CanInspectModule() => !Editing;
        private bool CanCreateModule() => !Editing;
        #endregion
    }
}
