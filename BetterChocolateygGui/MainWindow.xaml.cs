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

namespace BetterChocolateygGui {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow {
        ObservableCollection<ProgramInfo> programs = new ObservableCollection<ProgramInfo>();


        public MainWindow() {
            InitializeComponent();
            programListView.DataContext = programs;
        }

        private async void Refresh()  {
            Choco choco = new Choco();
            programs.Clear();
            List<ProgramInfo> data = choco.List(localOnly: true);
            foreach() {

            }
        }


    }
}
