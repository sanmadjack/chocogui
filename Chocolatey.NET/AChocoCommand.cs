using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocolatey {
    public abstract class AChocoCommand<T> {
        public event EventHandler<CommandCompleteEventArgs<T>> OnCommandComplete;

        public async void Run() {
            CommandCompleteEventArgs<T> output = new CommandCompleteEventArgs<T>();
            try {
                StringBuilder cmd = new StringBuilder(Choco.CHOCO_COMMAND);
                output.Result = await RunInternal(cmd);
            } catch (Exception e) {
                output.Error = e;
            }

            if (OnCommandComplete != null)
                OnCommandComplete(this, output);
        }

        protected abstract Task<T> RunInternal(StringBuilder cmd);

        protected void RunCommand(String command) {

        }




    }

    public class CommandCompleteEventArgs<T> {
        public virtual T Result { get; internal set; }
        public Exception Error { get; internal set; }
    }
}
