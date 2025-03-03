using NetForge.Core;
using ShareLingo.WinUI.ViewModel.Component;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace ShareLingo.WinUI.Components
{
    public class FileDialogPickerHelper
    {
        #region Methods
        public async Task<OpenFileDialogViewModel> OpenFile(string filter, string? initialDirectory = null, bool multiple = false)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            var filters = ParseDialogFilter(filter);
            foreach (var _filter in filters) picker.FileTypeFilter.Add(_filter);
            if (multiple)
            {
                var files = await picker.PickMultipleFilesAsync();
                return new OpenFileDialogViewModel(files.Select(x => x.Path).ToArray());
            }
            else
            {
                var file = await picker.PickSingleFileAsync();
                return new OpenFileDialogViewModel(file.Path);
            }
        }
        public Task<SaveFileDIalogViewModel> SaveFile(string filter, string? initialDirectory = null, string? filename = null)
        {
            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            var filters = ParseDialogFilter(filter);
            foreach (var _filter in filters)
                picker.FileTypeChoices.Add(_filter);
            if (multiple)
            {
                var files = await picker.PickMultipleFilesAsync();
                return new OpenFileDialogViewModel(files.Select(x => x.Path).ToArray());
            }
            else
            {
                var file = await picker.PickSingleFileAsync();
                return new OpenFileDialogViewModel(file.Path);
            }
        }
        public string[] ParseDialogFilter(string filter)
        {
            var matches = Regex.Matches(filter, @"\*\.[a-zA-Z0-9*]+");
            var extensions = matches.Cast<Match>().Select(m => m.Value).Distinct().ToArray();
            return extensions;
        }
        #endregion
    }
}
