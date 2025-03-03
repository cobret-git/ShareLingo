using ShareLingo.WinUI.ViewModel.Component;

namespace ShareLingo.WinUI.Services
{
    public interface IMediaManager
    {
        MediaData ImportCourseImage(string filePath);
        MediaData GetCourseImageData(string relativePath);
        MediaData ImportModuleImage(string filePath);
        MediaData GetModuleImage(string relativePath);
    }
}
