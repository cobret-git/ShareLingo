using System;
using System.IO.Abstractions;

namespace ShareLingo.WinUI.Services
{
    public interface IBuildInfoManager
    {
        #region Properties
        Version Version { get; }
        IFileSystem FileSystem { get; }
        string MediaDirectoryPath { get; }
        #endregion

        #region Methods
        bool IsDebug();
        #endregion
    }
}
