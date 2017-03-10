using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chocolatey
{
    public abstract class AChocoCommand<T>
    {
        private  Version CompatableChocoVersion = new Version(0,10,3);

        public event EventHandler<CommandCompleteEventArgs<T>> OnCommandComplete;
        public event EventHandler<CommandUpdatedEventArgs> OnCommandUpdated;
        public event EventHandler<CommandRunningEventArgs> OnCommandRunning;

        private bool CommandRan = false;
        private Process p = new Process();
        private Regex InputRequestRegex = new Regex(@"^(.+)\((.*\[.+\].*\))\:$");
        private Regex InputOptionRegex = new Regex(@"^(.*)\[(.)\](.*)$");

        protected AChocoCommand()
        {
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = FindExePath(Choco.CHOCO_COMMAND);
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            //p.StartInfo.RedirectStandardInput = true;

            p.OutputDataReceived += P_OutputDataReceived;
            p.Exited += P_Exited;
            p.ErrorDataReceived += P_ErrorDataReceived;


        }

        public async Task<T> Run()
        {
            if (CommandRan)
                throw new Exception("This command has already been run");
            CommandRan = true;

            CommandCompleteEventArgs<T> output = new CommandCompleteEventArgs<T>();
            try
            {
                output.Result = await RunInternal();
            }
            catch (Exception e)
            {
                output.Error = e;
            }

            if (OnCommandComplete != null)
                OnCommandComplete(this, output);

            return output.Result;
        }

        protected abstract Task<T> RunInternal();

        protected async Task RunCommand(String arguments)
        {
            await Task.Run(() => {
                p.StartInfo.Arguments = arguments;
                if (OnCommandRunning != null)
                    OnCommandRunning(this, new CommandRunningEventArgs() { StartInfo = p.StartInfo });
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
            });
        }

        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.Out.WriteLine(e.Data);
        }

        private void P_Exited(object sender, EventArgs e)
        {
            if (OnCommandComplete != null)
            {
                CommandCompleteEventArgs<T> ev = new CommandCompleteEventArgs<T>();
                OnCommandComplete(this,ev);
            }
        }

        private Version ChocolateyVersion;

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            if(e.Data.StartsWith("Chocolatey v"))
            {
                ChocolateyVersion = Version.Parse(e.Data.Substring(12));
                if (ChocolateyVersion > CompatableChocoVersion)
                    throw new NotSupportedException("");
                return;
            } else if(InputRequestRegex.IsMatch(e.Data)) 
            {
                //Do you want to run the script?([Y]es/[N]o/[P]rint):
                Match m = InputRequestRegex.Match(e.Data);

                CommandInputNeededEventArgs ea = new CommandInputNeededEventArgs();
                ea.Message = m.Groups[1].Value;
                foreach(String option in m.Groups[2].Value.Split('/'))
                {
                    if (!InputOptionRegex.IsMatch(option))
                        throw new Exception("Input request option does not match expected pattern: " + option);

                    Match om = InputOptionRegex.Match(option);
                    String character = om.Groups[2].Value;
                    StringBuilder text = new StringBuilder();
                    for (int i = 1; i< om.Groups.Count; i++){
                        text.Append(om.Groups[i].Value);
                    }

                }
            } else
            {
                if (OnCommandUpdated != null)
                {
                    CommandUpdatedEventArgs args = new CommandUpdatedEventArgs();
                    args.OutputLine = e.Data;
                    OnCommandUpdated(this, args);
                }
            }
        }

        /// <summary>
        /// Expands environment variables and, if unqualified, locates the exe in the working directory
        /// or the evironment's path.
        /// </summary>
        /// <param name="exe">The name of the executable file</param>
        /// <returns>The fully-qualified path to the file</returns>
        /// <exception cref="System.IO.FileNotFoundException">Raised when the exe was not found</exception>
        protected static string FindExePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                            return Path.GetFullPath(path);
                    }
                }
                throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }
            return Path.GetFullPath(exe);
        }
    }

    public class CommandUpdatedEventArgs : EventArgs
    {
        public String OutputLine { get; internal set; }
    }

    public class CommandCompleteEventArgs<T> : EventArgs
    {
        public virtual T Result { get; internal set; }
        public Exception Error { get; internal set; }
    }

    public class CommandRunningEventArgs: EventArgs
    {
        public ProcessStartInfo StartInfo;
    }

    public class CommandInputNeededEventArgs: EventArgs
    {
        public String Message = String.Empty;
        public Dictionary<Char, String> Options = new Dictionary<char, string>();
        public String Response = String.Empty;
    }
}
