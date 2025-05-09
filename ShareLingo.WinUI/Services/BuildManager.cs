using NetForge.Core;
using NetForge.WinUI;
using ShareLingo.Core.Services;
using System;
using System.IO.Abstractions;

namespace ShareLingo.WinUI.Services
{
    public class BuildManager : IBuildInfoManager
    {
        #region Constructors
        public BuildManager(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
            FilePicker = new FilePicker();

            DatabasePath = IsDebug()
                ? fileSystem.Path.Combine("C:\\Users\\konst\\source\\repos\\ShareLingo\\ShareLingo.WinUI\\bin\\x64\\Debug\\net8.0-windows10.0.19041.0\\win-x64", "Data", "client.db")
                : fileSystem.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Data", "\\client.db");
            LogDirectory = IsDebug()
                ? fileSystem.Path.Combine("C:\\Users\\konst\\source\\repos\\ShareLingo\\ShareLingo.WinUI\\bin\\x64\\Debug\\net8.0-windows10.0.19041.0\\win-x64", "Logs")
                : fileSystem.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Logs");

            if (!fileSystem.Directory.Exists(fileSystem.Path.GetDirectoryName(DatabasePath)!)) fileSystem.Directory.CreateDirectory(fileSystem.Path.GetDirectoryName(DatabasePath)!);
            if (!fileSystem.Directory.Exists(LogDirectory)) fileSystem.Directory.CreateDirectory(LogDirectory);
        }
        #endregion

        #region Properties
        public Version Version { get => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!; }
        public IFileSystem FileSystem { get; }
        public IFilePicker FilePicker { get; }
        public string DatabasePath { get; }
        public string LogDirectory { get; }
        #endregion

        #region Methods
        public bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
        #endregion
    }
}
