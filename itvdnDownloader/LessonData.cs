using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace itvdnDownloader
{
    public class LessonData : INotifyPropertyChanged
    {
        private string m_status = "";
        private int m_progress = 0;
        private bool m_isSelected = false;


        public string Url { get; set; }
        public string Title { get; set; }
        public string Number { get; set; }
        public string ManifestUrl { get; set; }

        public string Status
        {
            get { return m_status; }
            set { m_status = value; PropertyChange(); }
        }

        public int Progress
        {
            get { return m_progress; }
            set { m_progress = value; PropertyChange(); }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set { m_isSelected = value; PropertyChange(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void PropertyChange([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }


    }
}
