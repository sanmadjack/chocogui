using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocolatey {
    public class ChocoListCommand : AChocoCommand<List<ProgramInfo>> {

        public String Filter { get; internal set; }
        public bool LocalOnly { get; internal set; }

        protected async override Task<List<ProgramInfo>> RunInternal(StringBuilder cmd) {
            await Task.Delay(0);
            cmd.Append(" list");


            if (!String.IsNullOrWhiteSpace(Filter)) {
                cmd.Append(" ");
                cmd.Append(Filter);
            }


            if(LocalOnly) 
                cmd.Append(" -l");


            List<ProgramInfo> output = new List<ProgramInfo>();



            return output;
        }


    }

    public class ChocoListUpdateEventArgs {
        public virtual ProgramInfo Progam { get; internal set; }
    }

}
