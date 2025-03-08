using NetForge.Core;
using ShareLingo.WinUI.ViewModel;
using ShareLingo.WinUI.ViewModel.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace ShareLingo.WinUI.Components
{
    public class FileDialogPickerHelper
    {
        #region Methods
        public async Task<OpenFileDialogViewModel> OpenFile(string filter, bool multiple = false)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            var filters = ParseOpenDialogFilter(filter);
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
        public async Task<SaveFileDialogViewModel> SaveFile(string filter, string? filename = null)
        {
            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.SuggestedFileName = filename;
            var filters = ParseSaveDialogFilter(filter);
            foreach (var key in filters.Keys)
                picker.FileTypeChoices.Add(key, filters[key]);
            var file = await picker.PickSaveFileAsync();
            return new SaveFileDialogViewModel(file?.Path, file != null);
        }
        public string[] ParseOpenDialogFilter(string filter)
        {
            var matches = Regex.Matches(filter, @"\*\.[a-zA-Z0-9*]+");
            var extensions = matches.Cast<Match>().Select(m => m.Value).Distinct().ToArray();
            return extensions;
        }
        public Dictionary<string, string[]> ParseSaveDialogFilter(string filter)
        {
            var result = new Dictionary<string, string[]>();
            var parts = filter.Split('|');

            for (int i = 0; i < parts.Length - 1; i += 2)
            {
                string description = parts[i].Trim();
                string extensions = parts[i + 1].Trim();

                result[description] = extensions.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }

            return result;
        }
        #endregion
    }
}
