using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocolatey
{
    public class Choco
    {
        internal const String CHOCO_COMMAND = "choco";

        private String chocoPath;

        public Choco(string chocoPath = null) {
            this.chocoPath = chocoPath;
        }

        public ChocoListCommand CreateListCommand(String filter = null, bool localOnly = false) {
            ChocoListCommand cmd = new ChocoListCommand();

            return cmd;
        }


    }
}
