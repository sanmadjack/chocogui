using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocolatey {
    public class ChocoListCommand : AChocoCommand<List<ListItem>> {
        public event EventHandler<ChocoListUpdateEventArgs> OnUpdate;
        public String Filter { get; internal set; }
        public bool LocalOnly { get; internal set; }
        public int? Page { get; internal set; }
        public int PageSize {get; internal set; }
        private List<ListItem> Output = new List<ListItem>();

        public ChocoListCommand()
        {
            this.OnCommandUpdated += ChocoListCommand_OnCommentUpdate;
        }

        private void ChocoListCommand_OnCommentUpdate(object sender, CommandUpdatedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(e.OutputLine))
                return;

            String[] parts = e.OutputLine.Split(' ');
            Version ver;
            if (Version.TryParse(parts[1], out ver)) {
                ListItem info = new ListItem();
                info.Name = parts[0];
                info.Version = ver;
                if(parts.Length>2&&parts[2]=="[Approved]")
                {
                    info.Approved = true;
                }
                if(parts.Length>3)
                {
                    info.Comment = parts[3];
                }

                Output.Add(info);
                if(OnUpdate!=null)
                {
                    ChocoListUpdateEventArgs args = new ChocoListUpdateEventArgs();
                    args.Program = info;
                    OnUpdate(this, args);
                }
            }
        }

        protected async override Task<List<ListItem>> RunInternal() {
            await Task.Delay(0);
            StringBuilder cmd = new StringBuilder();
            cmd.Append("list");


            if (!String.IsNullOrWhiteSpace(Filter)) {
                cmd.Append(" ");
                cmd.Append(Filter);
            }


            if(LocalOnly) 
                cmd.Append(" -l");

            if(Page.HasValue)
            {
                cmd.Append(" --page=");
                cmd.Append(Page.Value.ToString());
            }

            cmd.Append(" --page-size=");
            cmd.Append(PageSize.ToString());


            List<ListItem> output = new List<ListItem>();

            await RunCommand(cmd.ToString());

            return output;
        }


    }

    public class ChocoListUpdateEventArgs {
        public virtual ListItem Program { get; internal set; }
    }

}
