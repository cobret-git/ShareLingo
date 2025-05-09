using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetForge.Core;
using ShareLingo.Core.Resources;
using ShareLingo.Core.Services;
using System.IO.Abstractions;
using FPR = ShareLingo.Core.Resources.FileProperties;
using CONTENT = ShareLingo.Core.Resources.Content;
using System.Globalization;

namespace ShareLingo.Core.ViewModel
{
    public partial class ModuleEditorViewModel : PageViewModelBase
    {
        #region Fields
        private readonly IFileSystem fileSystem;
        private readonly IFilePicker filePicker;
        private readonly IDataManager dataManager;
        private ModuleItemViewModel? originModule;
        [ObservableProperty] private string theoryText = string.Empty;
        [ObservableProperty] private bool infoBarOpened = false;
        [ObservableProperty] private string infoBarMessage = string.Empty;
        [ObservableProperty, NotifyCanExecuteChangedFor(nameof(DeleteDocumentCommand))] private bool canDeleteDocument;
        [ObservableProperty, NotifyCanExecuteChangedFor(nameof(DeleteCoverCommand))] private bool canDeleteCover;
        #endregion

        #region Constructors
        public ModuleEditorViewModel(IEventAggregator eventAggregator, IDataManager dataManager, IBuildInfoManager buildManager)
            : base(eventAggregator)
        {
            this.dataManager = dataManager;
            this.fileSystem = buildManager.FileSystem;
            this.filePicker = buildManager.FilePicker;

            Header = CONTENT.moduleEditor_header;
        }
        #endregion

        #region Properties
        public ushort MaxModuleNameLength { get => 256; }
        public string ModuleName { get => Module.Item.Name; set { Module.Item.Name = value?.Trim()!; OnPropertyChanged(); } }
        public string ModuleIndex { get => Module.Item.Index.ToString(); set { if (int.TryParse(value, NumberStyles.Integer, null, out var number)) { Module.Item.Index = Math.Max(1, number); OnPropertyChanged(); } } }
        public override IViewModelDataParameter? DataParameter { get => Module; set => SetModule(value); }
        public ModuleItemViewModel Module { get; private set; } = null!;
        #endregion

        #region Methods
        public override void Dispose()
        {
            Module?.Dispose();
            originModule?.Dispose();
            Module = null!;
            originModule = null!;
            IsDisposed = true;
        }
        [RelayCommand] private async Task ImportCover()
        {
            try
            {
                var result = await filePicker.OpenAsync(FPR.image_dlgFilter, DirectoryLocation.Desktop, false);
                if (!result.Success) return;
                if (Module.Cover != null) Module.Cover.Dispose();
                var fileName = fileSystem.Path.GetFileName(result.FileName);
                var stream = fileSystem.File.OpenRead(result.FileName);
                Module.Cover = new AttachedFileViewModel(fileName, stream);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
            finally { CanDeleteCover = Module.Cover != null; }
        }
        [RelayCommand] private async Task ImportDocument()
        {
            try
            {
                var result = await filePicker.OpenAsync(FPR.markdown_dlgFilter, DirectoryLocation.Desktop, false);
                if (!result.Success) return;
                if (Module.Theory != null) Module.Theory.Dispose();
                var fileName = fileSystem.Path.GetFileName(result.FileName);
                var stream = fileSystem.File.OpenRead(result.FileName);
                Module.Theory = new AttachedFileViewModel(fileName, stream);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
            finally { CanDeleteCover = Module.Theory != null; }
        }
        [RelayCommand(CanExecute = nameof(CanDeleteDocument))] private void DeleteDocument()
        {
            try
            {
                if (Module.Theory != null) Module.Theory.Dispose();
                Module.Theory = null;
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand(CanExecute = nameof(CanDeleteCover))] private void DeleteCover()
        {
            try
            {
                if (Module.Cover != null) Module.Cover.Dispose();
                Module.Cover = null;
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand] private void Save()
        {
            try
            {
                if (Module == null) throw new ArgumentNullException(nameof(Module));
                else if (originModule == null) throw new ArgumentNullException(nameof(originModule));
                if (string.IsNullOrWhiteSpace(Module.Item.Name))
                {
                    InfoBarMessage = CONTENT.moduleEditor_error_emptyModuleName;
                    InfoBarOpened = true;
                    return;
                }
                InfoBarOpened = false;
                originModule.Merge(Module);
                dataManager.SaveModuleData(originModule);
                var closeRequest = new PageNavigationRequest(typeof(CourseEditorViewModel), NavigationRequestAction.GoBack, originModule);
                eventAggregator.Publish(closeRequest);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Fatal(ex, CONTENT.log_error_courseEditor_saveFailedUnhandled)); }
        }
        [RelayCommand] private void Cancel()
        {
            try
            {
                var closeRequest = new PageNavigationRequest(typeof(CourseEditorViewModel), NavigationRequestAction.GoBack, originModule);
                eventAggregator.Publish(closeRequest);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Error(ex, CONTENT.log_error_courseEditor_cancelFailedUnhandled)); }
        }
        #endregion

        #region Helpers
        private void SetModule(IViewModelDataParameter? value)
        {
            try
            {
                if (value is not ModuleItemViewModel module) throw new ArgumentException($"Invalid data parameter type: {value?.GetType().Name}", nameof(value));
                originModule = module;
                Module = module.Clone();
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        #endregion
    }
}
