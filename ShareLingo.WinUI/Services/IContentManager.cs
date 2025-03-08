using Microsoft.UI.Xaml;
using NetForge.Core;
using ShareLingo.WinUI.Components;
using ShareLingo.WinUI.ViewModel;
using ShareLingo.WinUI.ViewModel.Component;
using System.Threading.Tasks;

namespace ShareLingo.WinUI.Services
{
    public interface IContentManager
    {
        Task<DefaultDialogViewModel> ShowWarn(string message);
        Task<DefaultDialogViewModel> ShowError(string message);
        Task<DefaultDialogViewModel> ShowInfo(string message);
        Task<DefaultDialogViewModel> ShowConfirm(string message, DialogButtons buttons);
        Task<PromptDialogViewModel> ShowPrompt(string message, IAttachedTextValidator? validator);

        Task<OpenFileDialogViewModel> OpenFile(string filter, bool multiple = false);
        Task<SaveFileDialogViewModel> SaveFile(string filter, string? filename = null);

        void InspectCourse(CourseContainerViewModel course);
        void InspectModule(ModuleItemViewModel module);
        void OpenModuleTrainer(ModuleItemViewModel module);
        void InspectExercise(ExerciseContainerViewModel exrcise);
        void OpenExerciseTrainer(ExerciseContainerViewModel exercise);
    }

    public class ContentManager : IContentManager
    {
        #region Fields
        private MessageDialogHelper messageHelper = new();
        private FileDialogPickerHelper filePickerHelper = new();
        #endregion

        #region Properties
        public XamlRoot MainWindowXamlRoot { get => messageHelper.XamlRoot; set => messageHelper.XamlRoot = value; }
        #endregion

        #region MessageDialog
        public Task<DefaultDialogViewModel> ShowWarn(string message) => messageHelper.ShowWarn(message);
        public Task<DefaultDialogViewModel> ShowError(string message) => messageHelper.ShowError(message);
        public Task<DefaultDialogViewModel> ShowInfo(string message) => messageHelper.ShowInfo(message);
        public Task<DefaultDialogViewModel> ShowConfirm(string message, DialogButtons buttons) => messageHelper.ShowConfirm(message, buttons);
        public Task<PromptDialogViewModel> ShowPrompt(string message, IAttachedTextValidator? validator) => messageHelper.ShowPrompt(message, validator);
        #endregion

        #region FileDialog
        public Task<OpenFileDialogViewModel> OpenFile(string filter, bool multiple = false) => filePickerHelper.OpenFile(filter, multiple);
        public Task<SaveFileDialogViewModel> SaveFile(string filter, string? filename = null) => filePickerHelper.SaveFile(filter, filename);
        #endregion
    }
}
