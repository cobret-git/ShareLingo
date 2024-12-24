using ShareLingo.Core.ViewModel.Component;

namespace ShareLingo.Core.Services
{
    public interface IContentManager
    {
        Task<DefaultDialogViewModel> ShowWarn(string title, string message);
        Task<DefaultDialogViewModel> ShowError(string message);
        Task<DefaultDialogViewModel> ShowInfo(string title, string message);
        Task<DefaultDialogViewModel> ShowConfirm(string message, DialogButtons buttons);
        Task<PromptDialogViewModel> ShowPrompt(string message, IPromptValidator? validator);

        Task<OpenFileDialogViewModel> OpenFile(string filter, string? initialDirectory = null, bool multiple = false);
        Task<SaveFileDIalogViewModel> SaveFile(string filter, string? initialDirectory = null, string? filename = null);

        void InspectCourse(CourseContainerViewModel course);
        void InspectModule(ModuleItemViewModel module);
        void OpenModuleTrainer(ModuleItemViewModel module);
        void InspectExercise(ExerciseContainerViewModel exrcise);
        void OpenExerciseTrainer(ExerciseContainerViewModel exercise);
    }
}
