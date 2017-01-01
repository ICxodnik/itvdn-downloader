using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itvdnDownloader
{
    public class ApplicationContext
    {
        public AuthContext AuthContext { get; set; } = new AuthContext();
        public MainDataContext MainDataContext { get; set; } = new MainDataContext();
    }
}
