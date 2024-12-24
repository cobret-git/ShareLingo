using ShareLingo.Core.ViewModel.Component;

namespace ShareLingo.Core.Services
{
    public interface IMediaManager
    {
        MediaData ImportCourseImage(string filePath);
        MediaData GetCourseImageData(string relativePath);
        MediaData ImportModuleImage(string filePath);
        MediaData GetModuleImage(string relativePath);
    }
}
