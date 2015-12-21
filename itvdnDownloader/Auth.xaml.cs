using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using mshtml;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace itvdnDownloader
{
    /// <summary>
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        private ItvdnWeb itvdnWeb = new ItvdnWeb();
        private AuthContext context = new AuthContext();

        public Auth()
        {
            InitializeComponent();
        }

        private async void btAuth_Click(object sender, RoutedEventArgs e)
        {
            var status = await itvdnWeb.Auth2(context.Login, context.Password);
            if (status)
            {
                AuthCompleted();
            }
            else
            {
                // todo: display authorization error message
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            context.Login = "ICxodnik@cbsid.com";
            context.Password = "lesenkaK***";
#endif
            this.DataContext = context;
        }

        private void AuthCompleted()
        {
            new MainWindow(context).Show();
            Close();
        }
    }
}
