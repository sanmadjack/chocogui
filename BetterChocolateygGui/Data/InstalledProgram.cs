using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chocolatey;

namespace BetterChocolateygGui
{
    public class ProgramInformation
    {
        public String Name
        {
            get
            {
                return programInfo.Name;
            }
        }

        public Version InstalledVersion
        {
            get { return programInfo.Version; }
        }
        public Version LatestVersion
        {
            get
            {
                if (updateInfo != null)
                    return updateInfo.LatestVersion;
                return programInfo.Version;
            }
        }

        public bool UpgradeAvailable
        {
            get
            {
                return LatestVersion > InstalledVersion;
            }
        }

        public bool IsSelected { get; set; }

        private Chocolatey.ListItem programInfo;
        public Chocolatey.Outdatedtem updateInfo;

        public ProgramInformation(Chocolatey.ListItem program)
        {
            this.programInfo = program;
            this.IsSelected = false;            
        }
       


    }
}
