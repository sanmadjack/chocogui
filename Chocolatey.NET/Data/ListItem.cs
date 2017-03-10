using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocolatey {
    public class ListItem {
        public String Name { get; internal set; }
        public Version Version { get; internal set; }
        public bool Approved { get; internal set; }
        public String Comment { get; internal set; }
    }
}
