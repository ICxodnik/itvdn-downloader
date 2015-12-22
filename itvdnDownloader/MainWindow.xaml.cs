using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
            context.Falling = true;
        }

        private async void btVideoList_Click(object sender, RoutedEventArgs e)
        {

            context.Falling = false;
            var lessons = await downloader.GetLessons(context.DataVideoPageUrl);
            foreach (var lesson in lessons)
            {
                context.Lessons.Add(lesson);
            }
            context.Falling = true;
        }

        private void main_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = context;
#if DEBUG
            context.DataVideoPageUrl = "http://itvdn.com/ru/video/csharp-essential";
#endif
            this.DataContext = context;
        }

        private void btChoose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btDownload_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
