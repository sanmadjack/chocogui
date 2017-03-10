using Chocolatey;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BetterChocolateygGui
{
    public class Model: ObservableCollection<ProgramInformation>, INotifyPropertyChanged
    {
        public Dispatcher DispatchTarget;

        public event EventHandler<ConsoleUpdatedEventArgs> ConsoleUpdated;

        private bool _UpgradesAvailable = false;
        public bool UpgradesAvailable
        {
            get { return _UpgradesAvailable; }
            set
            {
                _UpgradesAvailable = value;
                NotifyPropertyChanged("UpgradesAvailable");
            }
        }
        public Model()
        {

        }

        protected override void ClearItems()
        {
            UpgradesAvailable = false;
            base.ClearItems();
        }

        #region Refresh chain
        public async Task Refresh() 
        {
            Clear();
            ChocoListCommand refreshCmd = Choco.CreateListCommand(localOnly: true);
            refreshCmd.OnUpdate += Cmd_OnListUpdate;
            refreshCmd.OnCommandUpdated += Cmd_OnCommandUpdated;
            refreshCmd.OnCommandRunning += Cmd_OnCommandRunning;
            await refreshCmd.Run();

            ChocoOutdatedCommand outdatedCmd = Choco.CreateOutdatedCommand();
            outdatedCmd.OnUpdate += OutdatedCmd_OnUpdate;
            outdatedCmd.OnCommandUpdated += Cmd_OnCommandUpdated;
            outdatedCmd.OnCommandRunning += Cmd_OnCommandRunning;
            await outdatedCmd.Run();
        }

        private void Cmd_OnListUpdate(object sender, ChocoListUpdateEventArgs e)
        {
            DispatchTarget.Invoke(() =>
            {
                this.Add(new ProgramInformation(e.Program));
            });
        }

        private void OutdatedCmd_OnUpdate(object sender, ChocoOutdatedUpdateEventArgs e)
        {
            DispatchTarget.Invoke(() =>
            { 
                    ProgramInformation candidate = null;
                    foreach (ProgramInformation prog in this)
                    {
                        if (prog.Name == e.Program.Name)
                        {
                            candidate = prog;
                            break;
                        }
                    }
                    if (candidate != null)
                    {
                        int i = this.IndexOf(candidate);
                        this.RemoveAt(i);
                        candidate.updateInfo = e.Program;
                        this.Insert(i, candidate);
                        if(candidate.UpgradeAvailable)
                    {
                        UpgradesAvailable = true;
                    }
                    }
            });
        }
        #endregion

        #region Upgrade Chain
        public async Task UpgradeAll()
        {
            List<String> packages = new List<string>();
            for(int i =0; i < this.Count; i++)
            {
                if (this[i].UpgradeAvailable)
                    packages.Add(this[i].Name);
            }

            ChocoUpgradeCommand upgradeCmd = Choco.CreateUpgradeCommand(packages.ToArray<String>());
            upgradeCmd.OnCommandRunning += Cmd_OnCommandRunning;
            upgradeCmd.OnCommandUpdated += Cmd_OnCommandUpdated;
            await upgradeCmd.Run();
        }

        public async Task Upgrade(ProgramInformation pi)
        {
            if (!pi.UpgradeAvailable)
                throw new Exception("Program does not have an upgrade available");
            string[] packages = { pi.Name };
            ChocoUpgradeCommand upgradeCmd = Choco.CreateUpgradeCommand(packages: packages);
            upgradeCmd.OnCommandRunning += Cmd_OnCommandRunning;
            upgradeCmd.OnCommandUpdated += Cmd_OnCommandUpdated;
            await upgradeCmd.Run();
        }

        #endregion

        private void Cmd_OnCommandRunning(object sender, CommandRunningEventArgs e)
        {
            if (ConsoleUpdated != null)
                ConsoleUpdated(sender, new ConsoleUpdatedEventArgs() { Output = String.Concat(Path.GetFileName(e.StartInfo.FileName), " " , e.StartInfo.Arguments) } );
        }


        private void Cmd_OnCommandUpdated(object sender, CommandUpdatedEventArgs e)
        {
            if (ConsoleUpdated != null)
                ConsoleUpdated(sender,new ConsoleUpdatedEventArgs() { Output = e.OutputLine });
        }


        #region INotify Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
    public class ConsoleUpdatedEventArgs: EventArgs
    {
        public String Output;
    }
}
