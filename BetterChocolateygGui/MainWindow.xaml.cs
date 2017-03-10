using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chocolatey;
using System.ComponentModel;

namespace BetterChocolateygGui {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow, INotifyPropertyChanged
    {
        private bool _Working = false;
        public bool IsNotWorking
        {
            get { return !_Working; }
        }
        public bool IsWorking
        {
            get { return _Working; }
            private set
            {
                _Working = value;
                NotifyPropertyChanged("IsWorking");
                NotifyPropertyChanged("IsNotWorking");
            }
        }

        private Model model
        {
            get
            {
                return (Model)this.DataContext;
            }
        }

        public bool AdminMode { get; set; }

        public MainWindow() {
            AdminMode = false;
            InitializeComponent();
            model.ConsoleUpdated += Model_ConsoleUpdated;
            model.DispatchTarget = programListView.Dispatcher;
        }

        private void Model_ConsoleUpdated(object sender, ConsoleUpdatedEventArgs e)
        {
            this.consoleOutput.Dispatcher.Invoke(() =>
            {
                this.consoleOutput.AppendText(e.Output + Environment.NewLine);
                this.consoleOutput.ScrollToEnd();
            });
        }

        private async void Refresh()  {
            IsWorking = true;
            try
            {
                await model.Refresh();
            } finally
            {
                IsWorking = false;
            }
        }



        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
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

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private async void upgradeAllButton_Click(object sender, RoutedEventArgs e)
        {
            IsWorking = true;
            try
            {
                await model.UpgradeAll();
            } finally
            {
                IsWorking = false;
            }
        }
    }
}
