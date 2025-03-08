using NetForge.Core;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Linq;

namespace ShareLingo.WinUI.Services
{
    public interface ILoggerManager
    {
        void Verbose(string message);
        void Debug(string message);
        void Debug(Exception exception, string? message = null);
    }
    public class LoggerManager : ILoggerManager
    {
        #region Fields
        private readonly IEventAggregator eventAggregator;
        private readonly IFileSystem fileSystem;
        private readonly string logDirName;
        private readonly string logArchiveName;
        #endregion

        #region Constructors
        public LoggerManager(IEventAggregator eventAggregator, IBuildInfoManager buildManager)
        {
            this.eventAggregator = eventAggregator;
            this.fileSystem = buildManager.FileSystem;

            logDirName = buildManager.LogDirectory;
            logArchiveName = fileSystem.Path.Combine(logDirName, "app-logs.zip");
            CreateDirectories();
            ArchiveOldLogFiles();


            var logFile = fileSystem.Path.Combine(logDirName, "app-log_.txt");
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.File(logFile, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
                .CreateLogger();
            Log.Verbose("Starting host");

            eventAggregator.SubscribeAction<LoggedData>(OnLoggedDataReceived);
        }
        #endregion

        #region Methods
        public void Verbose(string message) => OnLoggedDataReceived(LoggedData.Trace(message));
        public void Debug(string message) => OnLoggedDataReceived(LoggedData.Debug(message));
        public void Debug(Exception exception, string? message = null) => OnLoggedDataReceived(LoggedData.Debug(exception, message));
        #endregion

        #region Handlers
        private void OnLoggedDataReceived(LoggedData data)
        {
            switch (data.Level)
            {
                case LogLevel.Debug: Serilog.Log.Logger.Debug(data.Exception, data.Message); break; 
                case LogLevel.Trace: Serilog.Log.Logger.Verbose(data.Message); break; 
                case LogLevel.Info: Serilog.Log.Logger.Information(data.Message); break; 
                case LogLevel.Warning: Serilog.Log.Logger.Warning(data.Message); break;
                case LogLevel.Error: Serilog.Log.Logger.Error(data.Exception, data.Message); break;
                case LogLevel.Fatal: Serilog.Log.Logger.Fatal(data.Exception, data.Message); break; 
                default: throw new NotImplementedException($"Unsupported value: {data.Level}");
            }
        }
        #endregion

        #region Helpers
        private void CreateDirectories()
        {
            if (!fileSystem.Directory.Exists(logDirName)) fileSystem.Directory.CreateDirectory(logDirName);
        }
        private void ArchiveOldLogFiles()
        {
            CreateLogARchiveIfNotExists();
            var oldFiles = GetOldFiles().ToArray();
            using var archiveFileStream = fileSystem.FileStream.New(logArchiveName, FileMode.Open, FileAccess.ReadWrite);
            using var archive = new ZipArchive(archiveFileStream, ZipArchiveMode.Update, leaveOpen: true);
            foreach (var oldFile in oldFiles)
            {
                var creationDate = fileSystem.File.GetCreationTime(oldFile);
                var entryFilePath = fileSystem.Path.Combine($"{creationDate.Year:D4}", $"{creationDate.Month:D2}", $"log_{creationDate.Day:D2}.txt");
                if (archive.Entries.Any(x => x.FullName == entryFilePath)) archive.GetEntry(entryFilePath)?.Delete();
                archive.CreateEntryFromFile(oldFile, entryFilePath, CompressionLevel.SmallestSize);
                fileSystem.File.Delete(oldFile);
            }
        }
        private void CreateLogARchiveIfNotExists()
        {
            if (fileSystem.File.Exists(logArchiveName)) return;
            using var fileStream = fileSystem.FileStream.New(logArchiveName, FileMode.Create, FileAccess.ReadWrite);
            using var zipStream = new ZipArchive(fileStream, ZipArchiveMode.Create, leaveOpen: true);
        }
        private IEnumerable<string> GetOldFiles()
        {
            var files = fileSystem.Directory.GetFiles(logDirName, "app-log_*.txt");
            foreach(var file in files)
            {
                var creationDate = fileSystem.File.GetCreationTime(file);
                creationDate = GetShortDate(creationDate);
                if (creationDate < DateTime.Today)
                    yield return file;
            }
        }
        private DateTime GetShortDate(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }
        #endregion
    }
}
