using System;
using System.IO.Abstractions;
using System.Reflection;

namespace ShareLingo.WinUI.Services
{
    public class BuildInfoManager : IBuildInfoManager
    {
        #region Constructors
        public BuildInfoManager()
        {
            FileSystem = new FileSystem();
            Version = Assembly.GetExecutingAssembly().GetName().Version!;

            var isDebug = IsDebug();
            var localDataDir = isDebug
                ? AppDomain.CurrentDomain.BaseDirectory
                : Windows.Storage.ApplicationData.Current.LocalFolder.Path;

            LogDirectory = FileSystem.Path.Combine(localDataDir, "Logs");
            DataDirectory = FileSystem.Path.Combine(localDataDir, "Data");
            MediaDirectory = FileSystem.Path.Combine(localDataDir, "Data", "Media");
        }
        #endregion

        #region Properties
        public Version Version { get;}
        public IFileSystem FileSystem { get;}
        public string MediaDirectory { get;}
        public string LogDirectory { get; }
        public string DataDirectory { get; }
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
