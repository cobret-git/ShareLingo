using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetForge.Core;
using ShareLingo.Core.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Abstractions;
using System.Threading.Tasks;
using CONTENT = ShareLingo.Core.Resources.Content;
using FPR = ShareLingo.Core.Resources.FileProperties;

namespace ShareLingo.Core.ViewModel
{
    public partial class CourseEditorViewModel : PageViewModelBase
    {
        #region Fields
        private readonly IDataManager dataManager;
        private readonly IFileSystem fileSystem;
        private readonly IFilePicker filePicker;
        private CourseContainerViewModel? originCourse;
        private CourseContainerViewModel course = new(new());
        [ObservableProperty] private bool infoBarOpened = false;
        [ObservableProperty] private string infoBarMessage = string.Empty;
        [ObservableProperty, NotifyCanExecuteChangedFor(nameof(DeleteDocumentCommand))] private bool canDeleteDocument;
        [ObservableProperty, NotifyCanExecuteChangedFor(nameof(DeleteCoverCommand))] private bool canDeleteCover;
        private CultureInfo selectedNativeCulture = null!;
        private CultureInfo selectedForeignCulture = null!;
        #endregion

        #region Constructors
        public CourseEditorViewModel(IEventAggregator eventAggregator, IDataManager dataManager, IBuildInfoManager buildManager)
            :base(eventAggregator)
        {
            this.dataManager = dataManager;
            this.fileSystem = buildManager.FileSystem;
            this.filePicker = buildManager.FilePicker;
            Header = CONTENT.courseEditor_header;

            Cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures).Where(x => x != CultureInfo.InvariantCulture).OrderBy(x => x.DisplayName).ToArray();
        }
        #endregion

        #region Properties
        public ushort MaxCourseNameLength { get => 256; }
        public ushort MaxAuthorNameLength { get => 256; }
        public string CourseName { get => Course.Item.Name; set { Course.Item.Name = value?.Trim()!; OnPropertyChanged(); } }
        public string AutrhorName { get => Course.Item.Author; set { Course.Item.Author = value?.Trim()!; OnPropertyChanged(); } }
        public CultureInfo SelectedNativeCulture { get => selectedNativeCulture; set { selectedNativeCulture = value; Course.Item.NativeLanguageCode = value.Name; OnPropertyChanged(); } }
        public CultureInfo SelectedForeignCulture { get => selectedForeignCulture; set { selectedForeignCulture = value; Course.Item.ForeignLanguageCode = value.Name; OnPropertyChanged(); } }
        public CourseContainerViewModel Course { get => course; private set { course = value; OnPropertyChanged(); } }
        public CultureInfo[] Cultures { get; }
        public override IViewModelDataParameter? DataParameter { get => originCourse; set { SetDataParameter(value); } }
        #endregion

        #region Methods
        [RelayCommand] private async Task ImportDocument()
        {
            try
            {
                var result = await filePicker.OpenAsync(FPR.markdown_dlgFilter, DirectoryLocation.Desktop, false);
                if (!result.Success) return;
                if (Course.Description != null) Course.Description.Dispose();
                var fileName = fileSystem.Path.GetFileName(result.FileName);
                var stream = fileSystem.File.OpenRead(result.FileName);
                Course.Description = new AttachedFileViewModel(fileName, stream);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Fatal(ex, CONTENT.log_error_courseEditor_importDocumentFailed)); }
            finally { CanDeleteCover = Course.Description != null; }
        }
        [RelayCommand(CanExecute = nameof(CanDeleteDocument))] private void DeleteDocument()
        {
            try
            {
                if (Course.Description != null) Course.Description.Dispose();
                Course.Description = null;
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Fatal(ex, CONTENT.log_error_courseEditor_deleteDocumentFailed)); }
        }
        [RelayCommand(CanExecute = nameof(CanDeleteCover))] private void DeleteCover()
        {
            try
            {
                if (Course.Cover != null) Course.Cover.Dispose();
                Course.Cover = null;
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Fatal(ex, CONTENT.log_error_courseEditor_deleteCoverFailed)); }
        }
        [RelayCommand] private async Task ImportCover()
        {
            try
            {
                var result = await filePicker.OpenAsync(FPR.image_dlgFilter, DirectoryLocation.Desktop, false);
                if (!result.Success) return;
                if (Course.Cover != null) Course.Cover.Dispose();
                var fileName = fileSystem.Path.GetFileName(result.FileName);
                var stream = fileSystem.File.OpenRead(result.FileName);
                Course.Cover = new AttachedFileViewModel(fileName, stream);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Fatal(ex, CONTENT.log_error_courseEditor_importCoverFailed)); }
            finally { CanDeleteCover = Course.Cover != null; }
        }
        [RelayCommand] private void Save()
        {
            try
            {
                if (Course == null) throw new ArgumentNullException(nameof(Course));
                else if (originCourse == null) throw new ArgumentNullException(nameof(originCourse));
                if (string.IsNullOrWhiteSpace(Course.Item.Name))
                {
                    InfoBarMessage = CONTENT.courseEditor_error_emptyCourseName;
                    InfoBarOpened = true;
                    return;
                }
                if (string.IsNullOrWhiteSpace(Course.Item.Author))
                {
                    InfoBarMessage = CONTENT.courseEditor_error_emptyAuthorName;
                    InfoBarOpened = true;
                    return;
                }
                InfoBarOpened = false;
                originCourse.Merge(Course);
                dataManager.SaveCourseData(originCourse);
                var closeRequest = new PageNavigationRequest(typeof(CourseEditorViewModel), NavigationAction.Back, originCourse);
                eventAggregator.Publish(closeRequest);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Fatal(ex, CONTENT.log_error_courseEditor_saveFailedUnhandled)); }
        }
        [RelayCommand] private void Cancel()
        {
            try
            {
                var closeRequest = new PageNavigationRequest(typeof(CourseEditorViewModel), NavigationAction.Back, originCourse);
                eventAggregator.Publish(closeRequest);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Error(ex, CONTENT.log_error_courseEditor_cancelFailedUnhandled)); }
        }
        public override void Dispose()
        {
            Course?.Dispose();
        }
        #endregion

        #region Helpers
        private void SetDataParameter(IViewModelDataParameter? value)
        {
            if (value is not CourseContainerViewModel courseVm) throw new ArgumentException($"Invalid data parameter type: {value?.GetType().Name}", nameof(value));
            originCourse = courseVm; 
            Course = courseVm.Clone()!;
            SelectedNativeCulture = courseVm.Item.NativeLanguageCode != null && Cultures.Any(x => x.Name == courseVm.Item.NativeLanguageCode)
                ? new CultureInfo(courseVm.Item.NativeLanguageCode)
                : new CultureInfo("en-US");
            SelectedForeignCulture = courseVm.Item.ForeignLanguageCode != null && Cultures.Any(x => x.Name == courseVm.Item.ForeignLanguageCode)
                ? new CultureInfo(courseVm.Item.ForeignLanguageCode)
                : new CultureInfo("en-US");
            CanDeleteCover = Course.Cover != null;
            CanDeleteDocument = Course.Description != null;
        }
        #endregion
    }
}
