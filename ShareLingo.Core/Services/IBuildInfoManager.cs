using NetForge.Core;
using System.IO.Abstractions;

namespace ShareLingo.Core.Services
{
    public interface IBuildInfoManager
    {
        #region Properties
        Version Version { get; }
        IFileSystem FileSystem { get; }
        IFilePicker FilePicker { get; }
        string MediaDirectory { get; }
        string LogDirectory { get; }
        string DataDirectory { get; }
        #endregion

        #region Methods
        bool IsDebug();
        #endregion
    }
}
