using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocolatey
{
    public abstract class Choco
    {
        internal const String CHOCO_COMMAND = "choco.exe";


        public static ChocoOutdatedCommand CreateOutdatedCommand()
        {
            ChocoOutdatedCommand cmd = new ChocoOutdatedCommand();
            return cmd;
        }

        public static ChocoListCommand CreateListCommand(String filter = null, bool localOnly = false, int? page = null, int pageSize = 25) {
            ChocoListCommand cmd = new ChocoListCommand();

            cmd.Filter = filter;
            cmd.LocalOnly = localOnly;
            cmd.Page = page;
            cmd.PageSize = pageSize;

            return cmd;
        }



        public static ChocoUpgradeCommand CreateUpgradeCommand(String[] packages)
        {
            ChocoUpgradeCommand cmd = new ChocoUpgradeCommand();
            cmd.Packages = packages;
            return cmd;
        }

    }
}
