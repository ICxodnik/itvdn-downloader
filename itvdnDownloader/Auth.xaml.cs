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
            context.Falling = false;
            try
            {
                var status = await itvdnWeb.Auth(context.Login, context.Password);

                if (status)
                {
                    AuthCompleted();
                }
            }
            catch
            {
                {
                    // todo: display authorization error message
                    MessageBoxResult result = MessageBox.Show("Authorization error, do you want to close this window?",
       "Confirmation", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        Close();
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        new MainWindow(context).Show();
                    }

                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            context.Falling = true;
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
