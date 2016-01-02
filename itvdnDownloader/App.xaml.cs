using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace itvdnDownloader
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string ApplicationName = "ITVDN Downloader";
        internal const string ContextFilename = "context";

        protected void OnStartup(object sender, StartupEventArgs e)
        {
            var context = LoadContext();
            var mainWindow = new MainWindow(context.MainDataContext);
            var authWindow = new Auth(context.AuthContext);
            var authResult = authWindow.ShowDialog();
            if (authResult.HasValue && authResult.Value)
            {
                mainWindow.Show();
            }
            else
            {
                mainWindow.Close();
            }
        }

        private ApplicationContext LoadContext()
        {
            var dataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                ApplicationName
            );
            Directory.CreateDirectory(dataPath);
            var contextFilePath = Path.Combine(dataPath, ContextFilename);

            ApplicationContext appContext = null;
            if (File.Exists(contextFilePath))
            {
                try
                {
                    appContext = JsonConvert.DeserializeObject<ApplicationContext>(contextFilePath);
                }
                catch (SerializationException)
                {
                    // todo: add some logging here
                }
            }
            return appContext ?? new ApplicationContext();
        }
    }
}
