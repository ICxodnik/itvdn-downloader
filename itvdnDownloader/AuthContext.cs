using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace itvdnDownloader
{
    public class AuthContext : INotifyPropertyChanged
    {
        private bool canAuth = true;

        public string Login { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public bool CanAuth
        {
            get
            {
                return canAuth;
            }
            set
            {
                canAuth = value;
                Notify(nameof(CanAuth));
            }
        }

        private void Notify(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
