using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocolatey {
    public class ChocoUpgradeCommand : AChocoCommand<List<ListItem>> {
        public event EventHandler<ChocoListUpdateEventArgs> OnUpdate;
        public String[] Packages { get; internal set; }
        public bool Noop { get; internal set; }

        private List<ListItem> Output = new List<ListItem>();

        public ChocoUpgradeCommand()
        {
            this.OnCommandUpdated += ChocoUpgradeCommand_OnCommandUpdated; ;
        }

        private void ChocoUpgradeCommand_OnCommandUpdated(object sender, CommandUpdatedEventArgs e)
        {
        }

        protected async override Task<List<ListItem>> RunInternal() {
            await Task.Delay(0);
            StringBuilder cmd = new StringBuilder();
            cmd.Append("upgrade");


            if (Packages.Length > 0)
            {
                foreach (String package in Packages)
                {
                    if (!String.IsNullOrWhiteSpace(package))
                    {
                        cmd.Append(" ");
                        cmd.Append(package);
                    }
                }
            } else
            {
                throw new Exception("At least one package name is required");
            }

            //cmd.Append(" --noop");

            List<ListItem> output = new List<ListItem>();

            await RunCommand(cmd.ToString());

            return output;
        }


    }

    public class ChocoUpgradeUpdateEventArgs {
        public virtual ListItem Progam { get; internal set; }
    }

}
