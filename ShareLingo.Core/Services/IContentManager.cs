using ShareLingo.Core.ViewModel.Component;

namespace ShareLingo.Core.Services
{
    public interface IContentManager
    {
        Task<DefaultDialogViewModel> ShowWarn(string title, string message);
        Task<DefaultDialogViewModel> ShowError(string title, string message);
        Task<DefaultDialogViewModel> ShowInfo(string title, string message);
        Task<DefaultDialogViewModel> ShowConfirm(string title, string message, DialogButtons buttons);
        Task<PromptDialogViewModel> ShowPrompt(string title, string message, IPromptValidator? validator);

        void InspectCourse(CourseContainerViewModel course);
        void InspectModule(ModuleItemViewModel module);
        void OpenModuleTrainer(ModuleItemViewModel module);
        void InspectExercise(ExerciseContainerViewModel exrcise);
        void OpenExerciseTrainer(ExerciseContainerViewModel exercise);
    }
}
