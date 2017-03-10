using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chocolatey {
    public class ChocoOutdatedCommand : AChocoCommand<List<Outdatedtem>> {
        public event EventHandler<ChocoOutdatedUpdateEventArgs> OnUpdate;

        private List<ListItem> Output = new List<ListItem>();

        private readonly Regex OutdatedOutputRegex = new Regex(@"([^\s]+)\|(.+)\|(.+)\|(.+)");

        public ChocoOutdatedCommand()
        {
            this.OnCommandUpdated += ChocoOutdatedCommand_OnCommandUpdated; ;
        }

        private void ChocoOutdatedCommand_OnCommandUpdated(object sender, CommandUpdatedEventArgs e)
        {
            if(OutdatedOutputRegex.IsMatch(e.OutputLine))
            {
                Match m = OutdatedOutputRegex.Match(e.OutputLine);
                Outdatedtem info = new Outdatedtem();
                info.Name = m.Groups[1].Value;
                info.InstalledVersion = Version.Parse(m.Groups[2].Value);
                info.LatestVersion= Version.Parse(m.Groups[3].Value);
                info.Pinned = Boolean.Parse(m.Groups[4].Value);

                if(OnUpdate!=null)
                {
                    ChocoOutdatedUpdateEventArgs args = new ChocoOutdatedUpdateEventArgs();
                    args.Program = info;
                    OnUpdate(this, args);
                }
            }
        }

        protected async override Task<List<Outdatedtem>> RunInternal() {
            await Task.Delay(0);
            StringBuilder cmd = new StringBuilder();
            cmd.Append("outdated");


            List<Outdatedtem> output = new List<Outdatedtem>();

            await RunCommand(cmd.ToString());

            return output;
        }


    }

    public class ChocoOutdatedUpdateEventArgs {
        public virtual Outdatedtem Program { get; internal set; }
    }

}
