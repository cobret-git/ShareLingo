using NetForge.Core;
using ShareLingo.Core.Services;
using System.IO.Abstractions;
using System.Windows;

namespace ShareLingo.WPF.Services
{
    public class BuildManager : IBuildInfoManager
    {
        #region Constructors
        public BuildManager()
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
            FileSystem = new FileSystem();
            Theme = ThemeVariant.Light; // Default theme
            FilePicker = new FilePickerService();

            DatabasePath = IsDebug()
                ? FileSystem.Path.Combine(Environment.CurrentDirectory, "Data", "local.db")
                : FileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "ShareLingo", "Data", "local.db");
            LogDirectory = IsDebug()
                ? FileSystem.Path.Combine(Environment.CurrentDirectory, "Logs")
                : FileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "ShareLingo", "Logs");

            if (IsDebug())
            {
                if (!FileSystem.Directory.Exists(LogDirectory)) FileSystem.Directory.CreateDirectory(LogDirectory);
                if (!FileSystem.Directory.Exists(FileSystem.Path.GetDirectoryName(DatabasePath)))
                    FileSystem.Directory.CreateDirectory(FileSystem.Path.GetDirectoryName(DatabasePath));
            }
        }
        #endregion

        #region Properties
        public Version Version { get; }
        public IFileSystem FileSystem { get; }
        public IFilePicker FilePicker { get; }
        public ThemeVariant Theme { get; private set; }
        public string DatabasePath { get; }
        public string LogDirectory { get; }
        #endregion

        #region Methods
        public void ApplyTheme(ThemeVariant theme)
        {
            //string themeName = theme.ToString();

            //// Clear existing theme resources
            //Application.Current.Resources.MergedDictionaries.Where(x => x.Source != null && x.Source.OriginalString.Contains("Theme")).ToList()
            //    .ForEach(x => Application.Current.Resources.MergedDictionaries.Remove(x));

            //var previousResources = Application.Current.Resources.MergedDictionaries.Select(x => x.Source).ToList();
            //Application.Current.Resources.MergedDictionaries.Clear();

            //// Create new resource dictionary
            //ResourceDictionary newTheme = new ResourceDictionary();

            //// Load the theme
            //switch (theme)
            //{
            //    case ThemeVariant.Light:
            //        newTheme.Source = new Uri("Assets/Themes/LightTheme.xaml", UriKind.Relative);
            //        break;
            //    case ThemeVariant.Dark:
            //        newTheme.Source = new Uri("Assets/Themes/DarkTheme.xaml", UriKind.Relative);
            //        break;
            //}

            //// Apply the theme
            //Application.Current.Resources.MergedDictionaries.Add(newTheme);
            //foreach (var prevRes in previousResources)
            //    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = prevRes });

            //Theme = theme;
        }
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
