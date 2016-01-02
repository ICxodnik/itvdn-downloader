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
using System.Runtime.InteropServices;
using System.Threading;

namespace itvdnDownloader
{
    /// <summary>
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        private ItvdnWeb itvdnWeb = new ItvdnWeb();
        private AuthContext context;

        public Auth(AuthContext authContext)
        {
            context = authContext;
            InitializeComponent();
        }

        private async void btAuth_Click(object sender, RoutedEventArgs e)
        {
            if (!context.CanAuth)
            {
                return;
            }
            context.CanAuth = false;
            try
            {
                var status = await itvdnWeb.Auth(context.Login, context.Password);
                if (status)
                {
                    AuthCompleted();
                }
                else
                {
                    MessageBox.Show("Email or/and password is incorrect", "Authorization error", MessageBoxButton.OK);
                    context.CanAuth = true;
                }
            }
            catch
            {
                MessageBox.Show($"Can't connect to {ItvdnWeb.AuthUrl}", "Authorization error", MessageBoxButton.OK);
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            context.Login = "ICxodnik@cbsid.com";
            context.Password = "lesenkaK***";
#endif
            DataContext = context;
        }

        private void AuthCompleted()
        {
            DialogResult = true;
            Close();
        }
    }
}
