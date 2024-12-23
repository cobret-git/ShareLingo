using Serilog;

namespace ShareLingo.Core.Services
{
    public interface ILoggerManager
    {
        void Verbose(string message);
        void Debug(string message);
        void Debug(Exception exception, string? message = null);
    }
    public class LoggerManager : ILoggerManager
    {
        #region Constructors
        public LoggerManager(IBuildInfoManager buildManager)
        {
            var fileSystem = buildManager.FileSystem;

            var verboseFile = buildManager.IsDebug()
                ? fileSystem.Path.Combine(Environment.CurrentDirectory, "Log", "verbose.txt")
                : fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Termin Repeater", "Log", "verbose.txt");
            var debugFile = buildManager.IsDebug()
                ? fileSystem.Path.Combine(Environment.CurrentDirectory, "Log", "debug.txt")
                : fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Termin Repeater", "Log", "debug.txt");

            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose().WriteTo.File(verboseFile, rollingInterval: RollingInterval.Day)
                .MinimumLevel.Debug().WriteTo.File(debugFile, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
        #endregion

        #region Methods
        public void Verbose(string message)
        {
            Log.Verbose(message);
        }
        public void Debug(string message)
        {
            Log.Debug(message);
        }
        public void Debug(Exception exception, string? message = null)
        {
            Log.Debug(exception, message ?? string.Empty);
        }
        #endregion
    }
}
