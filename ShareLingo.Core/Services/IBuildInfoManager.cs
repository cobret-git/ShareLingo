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
        ThemeVariant Theme { get; }
        /// <summary>
        /// The path to the database file.
        /// </summary>
        string DatabasePath { get; }
        string LogDirectory { get; }
        #endregion

        #region Methods
        bool IsDebug();
        void ApplyTheme(ThemeVariant theme);
        #endregion
    }
    public enum ThemeVariant
    {
        Light,
        Dark
    }
}
