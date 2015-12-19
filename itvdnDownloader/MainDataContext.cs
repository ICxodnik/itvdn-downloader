using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itvdnDownloader
{
    class MainDataContext
    {
        public string DataVideoPageUrl { get; set; }
        public string DataVideoLocFolder { get; set; }
        public AuthContext Auth { get; set; }
        public ObservableCollection<LessonData> Lessons { get; set; }
    }
}
