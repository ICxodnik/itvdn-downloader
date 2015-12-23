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

namespace itvdnDownloader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainDataContext context = new MainDataContext();
        private ItvdnWeb downloader = new ItvdnWeb();

        public MainWindow(AuthContext authContext)
        {
            InitializeComponent();
            context.Auth = authContext;
        }

        private async void btVideoList_Click(object sender, RoutedEventArgs e)
        {
            if (!context.CanReadSourse)
            {
                return;
            }
            context.CanReadSourse = false;
          //  await Task.Delay(5000);
            var lessons = await downloader.GetLessons(context.DataVideoPageUrl);
            foreach (var lesson in lessons)
            {
                context.Lessons.Add(lesson);
            }
            context.CanReadSourse = true;
        }

        private void main_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = context;
#if DEBUG
            context.DataVideoPageUrl = "http://itvdn.com/ru/video/csharp-essential";
#endif
        }

        private void btChoose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btDownload_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
