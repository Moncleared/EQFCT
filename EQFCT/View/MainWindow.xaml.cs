using CommonServiceLocator;
using EQFCT.Helper;
using EQFCT.ViewModel;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EQFCT.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            taskBarIcon.LeftClickCommand = TaskBarLeftClickCommand;
            taskBarIcon.NoLeftClickDelay = true;
        }

        private RelayCommand fTaskBarLeftClickCommand;
        public RelayCommand TaskBarLeftClickCommand
        {
            get
            {
                if (fTaskBarLeftClickCommand == null)
                {
                    fTaskBarLeftClickCommand = new RelayCommand( 
                        ()=> {
                            if (this.WindowState == WindowState.Minimized)
                                this.WindowState = WindowState.Normal;
                            this.Focus();
                        });
                }
                return fTaskBarLeftClickCommand;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized )
            {
                taskBarIcon.Visibility = Visibility.Visible;
                this.ShowInTaskbar = false;
            }
            else
            {
                taskBarIcon.Visibility = Visibility.Hidden;
                this.ShowInTaskbar = true;
            }
        }

        private void BidTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BidTextBox.ScrollToEnd();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var vVM = ServiceLocator.Current.GetInstance<MainViewModel>();
            vVM.AppendText("Checking for update...");
            try
            {
                using (var mgr = new UpdateManager("https://opendkp-publisher.s3.us-east-2.amazonaws.com/eqfct"))
                {
                    
                    var vSomething = await mgr.CheckForUpdate();
                    if ( vSomething.ReleasesToApply.Any() )
                    {
                        vVM.AppendText("Update available...");

                        var vAppUpdated = await mgr.UpdateApp();
                        vVM.AppendText("Update has completed...please restart for the latest version...");

                    }
                    else
                    {
                        vVM.AppendText("No updates published...");
                    }
                }
            }
            catch (Exception vException)
            {
                vVM.AppendText("Error checking for app update!...");
                vVM.AppendText(string.Format("{0}", vException.Message));
            }
        }
    }
}
