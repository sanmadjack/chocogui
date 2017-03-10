using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocolatey {
    public class Outdatedtem {
        public String Name { get; internal set; }
        public Version InstalledVersion { get; internal set; }
        public Version LatestVersion { get; internal set; }
        public bool Pinned { get; internal set; }
    }
}
