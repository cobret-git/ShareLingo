using ShareLingo.Core.Services;
using ShareLingo.Core.ViewModel.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLingo.Wpf.Service
{
    public class ContentManager : IContentManager
    {
        public void InspectCourse(CourseContainerViewModel course)
        {
            throw new NotImplementedException();
        }
        public void InspectExercise(ExerciseContainerViewModel exrcise)
        {
            throw new NotImplementedException();
        }
        public void InspectModule(ModuleItemViewModel module)
        {
            throw new NotImplementedException();
        }
        public void OpenExerciseTrainer(ExerciseContainerViewModel exercise)
        {
            throw new NotImplementedException();
        }

        public Task<OpenFileDialogViewModel> OpenFile(string filter, string? initialDirectory = null, bool multiple = false)
        {
            throw new NotImplementedException();
        }

        public void OpenModuleTrainer(ModuleItemViewModel module)
        {
            throw new NotImplementedException();
        }

        public Task<SaveFileDIalogViewModel> SaveFile(string filter, string? initialDirectory = null, string? filename = null)
        {
            throw new NotImplementedException();
        }

        public Task<DefaultDialogViewModel> ShowConfirm(string message, DialogButtons buttons)
        {
            throw new NotImplementedException();
        }

        public Task<DefaultDialogViewModel> ShowError(string message)
        {
            throw new NotImplementedException();
        }

        public Task<DefaultDialogViewModel> ShowInfo(string title, string message)
        {
            throw new NotImplementedException();
        }

        public Task<PromptDialogViewModel> ShowPrompt(string message, IPromptValidator? validator)
        {
            throw new NotImplementedException();
        }

        public Task<DefaultDialogViewModel> ShowWarn(string title, string message)
        {
            throw new NotImplementedException();
        }
    }
}
