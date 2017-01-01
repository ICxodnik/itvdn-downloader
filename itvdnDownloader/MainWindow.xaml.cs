using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;
using System.IO;

namespace itvdnDownloader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainDataContext context;
        private ItvdnWeb downloader = new ItvdnWeb();
        private BatchDownloader m_downloader;

        public MainWindow(MainDataContext mainContext)
        {
            context = mainContext;
            InitializeComponent();

            this.Loaded += main_Loaded;
        }

        private async void btVideoList_Click(object sender, RoutedEventArgs e)
        {
            if (context.CanReadSourse)
            {
                context.CanReadSourse = false;
                var lessons = await downloader.GetLessons(context.DataVideoPageUrl);
                foreach (var lesson in lessons)
                {
                    context.Lessons.Add(lesson);
                }
                context.CanReadSourse = true;
            }
        }

        private void main_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            context.DataVideoPageUrl = "http://itvdn.com/ru/video/csharp-essential";
            context.DataVideoLocFolder = @"E:\TEMP\ITVDN_Downloader";
#endif
            this.DataContext = context;
        }

        private void btChoose_Click(object sender, RoutedEventArgs e)
        {
            var folderChoose = new VistaFolderBrowserDialog();
            folderChoose.RootFolder = Environment.SpecialFolder.MyDocuments;
            var path = Environment.GetFolderPath(folderChoose.RootFolder);
            var dialogResult = folderChoose.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                var choosedFolder = folderChoose.SelectedPath;
                if (Directory.Exists(choosedFolder))
                {
                    context.DataVideoLocFolder = choosedFolder;
                }
            }
        }

        private async void btDownload_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(context.DataVideoLocFolder))
            {
                return;
            }

            var list = context.Lessons.Where(x => x.IsSelected).ToList();
            m_downloader = new BatchDownloader(context.DataVideoLocFolder);

            try
            {
                await m_downloader.Download(list);
            }
            catch(TaskCanceledException cancelException)
            {

            }
        }

        private void main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_downloader != null)
            {
                m_downloader.Stop();
            }
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var lesson in context.Lessons)
            {
                lesson.IsSelected = true;
            }
        }

        private void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var lesson in context.Lessons)
            {
                lesson.IsSelected = false;
            }
        }
    }
}
