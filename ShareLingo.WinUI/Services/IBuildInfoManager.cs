using System;
using System.IO.Abstractions;
using System.Reflection;

namespace ShareLingo.WinUI.Services
{
    public interface IBuildInfoManager
    {
        #region Properties
        Version Version { get; }
        IFileSystem FileSystem { get; }
        string MediaDirectoryPath { get; }
        string LogDirectory { get; }
        #endregion

        #region Methods
        bool IsDebug();
        #endregion
    }
    public class BuildInfoManager : IBuildInfoManager
    {
        #region Constructors
        public BuildInfoManager()
        {
            FileSystem = new FileSystem();
            Version = Assembly.GetExecutingAssembly().GetName().Version!;

            var isDebug = IsDebug();
            var localDataDir = isDebug
                ? Environment.CurrentDirectory
                : Windows.Storage.ApplicationData.Current.LocalFolder.Path;

            LogDirectory = FileSystem.Path.Combine(localDataDir, "Logs");
        }
        #endregion

        #region Properties
        public Version Version { get;}
        public IFileSystem FileSystem { get;}
        public string MediaDirectoryPath { get;}
        public string LogDirectory { get; }
        #endregion

        #region Methodss
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
