using Microsoft.Win32;
using NetForge.Core;
using System.IO;

namespace ShareLingo.WPF.Services
{
    public class FilePickerService : IFilePicker
    {
        public Task<OpenFileDialogResult> OpenAsync(string filter, DirectoryLocation directory = DirectoryLocation.Unspecified, bool multiple = false)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = filter,
                Multiselect = multiple,
                InitialDirectory = ConvertToPath(directory)
            };
            bool? dialogResult = dialog.ShowDialog();
            var result = new OpenFileDialogResult() { FileNames = dialog.FileNames, FileName = dialog.FileName, Success = dialogResult == true };
            return Task.FromResult(result);
        }
        public Task<SaveFileDialogResult> SaveAsync(string filter, DirectoryLocation directory = DirectoryLocation.Unspecified, string? fileName = null)
        {
            var dialog = new SaveFileDialog()
            {
                Filter = filter,
                InitialDirectory = ConvertToPath(directory),
                FileName = fileName
            };
            bool? dialogResult = dialog.ShowDialog();
            var result = new SaveFileDialogResult() { FileName = dialog.FileName!, Success = dialogResult == true };
            return Task.FromResult(result);
        }
        public string ConvertToPath(DirectoryLocation location)
        {
            switch (location)
            {
                case DirectoryLocation.Unspecified: return string.Empty;
                case DirectoryLocation.Documents: return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                case DirectoryLocation.Computer: return Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
                case DirectoryLocation.Desktop: return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                case DirectoryLocation.Downloads: return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                case DirectoryLocation.Home: return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                case DirectoryLocation.Music: return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                case DirectoryLocation.Pictures: return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                case DirectoryLocation.Videos: return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                case DirectoryLocation.Objects3D: return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                default: return string.Empty;
            }
        }
    }
}
