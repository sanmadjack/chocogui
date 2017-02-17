using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocolatey {
    internal class CommandBuilder {
        private const String CHOCO_COMMAND = "choco";

        private ChocoCommands command;
        private List<String> args = new List<string>();

        private StringBuilder commandString = new StringBuilder(CHOCO_COMMAND + " ");

        public CommandBuilder(ChocoCommands command) {
            this.command = command;
        }

        public void AddArgument(String name) {
            this.args.Add(name);
        }

        public String Create() {
            StringBuilder output = new StringBuilder(CHOCO_COMMAND);
            output.Append(" ");
            output.Append(command.ToString());

            foreach(String arg in args) {

            }

            return output.ToString();
        }
        
    }
}
