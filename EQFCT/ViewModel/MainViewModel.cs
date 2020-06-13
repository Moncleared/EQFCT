using CommonServiceLocator;
using EQFCT.Model;
using EQFCT.Service;
using EQFCT.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Squirrel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EQFCT.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        EqLogService fEqLogService = new EqLogService();
        WindowService fWindowService = new WindowService();
        bool fWindowsLocked = true;

        private DmgDoneViewModel fDmgDoneViewModel;
        public DmgDoneViewModel DmgDoneViewModel
        {
            get
            {
                return fDmgDoneViewModel;
            }
            set
            {
                if ( value != fDmgDoneViewModel )
                {
                    fDmgDoneViewModel = value;
                    RaisePropertyChanged("DmgDoneViewModel");

                }
            }
        }

        private DmgTakenViewModel fDmgTakenViewModel;
        public DmgTakenViewModel DmgTakenViewModel
        {
            get
            {
                return fDmgTakenViewModel;
            }
            set
            {
                if (value != fDmgTakenViewModel)
                {
                    fDmgTakenViewModel = value;
                    RaisePropertyChanged("DmgTakenViewModel");

                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            fWindowService.InitializeWindows();
            DmgDoneViewModel = ServiceLocator.Current.GetInstance<DmgDoneViewModel>();
            DmgTakenViewModel = ServiceLocator.Current.GetInstance<DmgTakenViewModel>();

            fWindowsLocked = true;
            LockText = "Unlock Windows";
            LogFile = Properties.Settings.Default.LogFile;
            if (!string.IsNullOrWhiteSpace(LogFile) && File.Exists(LogFile) ) this.StartLogMonitoring();
            Messenger.Default.Register<GenericMessage>(
                this, (action) => RecieveMessage(action)
                );
        }

        private string fLockText;
        public string LockText
        {
            get { return fLockText; }
            set
            {
                if (value != fLockText)
                {
                    fLockText = value;
                    RaisePropertyChanged("LockText");
                }
            }
        }

        private string fLogFile;
        public string LogFile
        {
            get { return fLogFile; }
            set
            {
                if (value != fLogFile)
                {
                    fLogFile = value;
                    Properties.Settings.Default.LogFile = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChanged("LogFile");
                }
            }
        }

        private RelayCommand fMonitorLogCommand;
        public RelayCommand MonitorLogCommand
        {
            get
            {
                if (fMonitorLogCommand == null)
                {
                    fMonitorLogCommand = new RelayCommand(this.StartLogMonitoring);
                }
                return fMonitorLogCommand;
            }
        }


        private void StartLogMonitoring()
        {
            fEqLogService.MonitorLog(LogFile);
        }

        private RelayCommand fBrowseCommand;
        public RelayCommand BrowseCommand
        {
            get
            {
                if (fBrowseCommand == null)
                {
                    fBrowseCommand = new RelayCommand(this.PopBrowseDialog);
                }
                return fBrowseCommand;
            }
        }

        private void PopBrowseDialog()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                LogFile = filename;
            }
        }

        private RelayCommand fDebugTest;
        public RelayCommand DebugTest
        {
            get
            {
                if (fDebugTest == null)
                {
                    fDebugTest = new RelayCommand(this.DoDebugTest);
                }
                return fDebugTest;
            }
        }
        private void DoDebugTest()
        {
            Random vRandom = new Random();
            var vFontSize = vRandom.Next(24, 64);
            Messenger.Default.Send<DmgDoneMessage>(new DmgDoneMessage() { Damage = new DmgModel() { Text = "111", Top = 0, FontSize = 24 } });
            Messenger.Default.Send<DmgDoneMessage>(new DmgDoneMessage() { Damage = new DmgModel() { Text = "222", Top = 24, FontSize = vFontSize } });
            Messenger.Default.Send<DmgDoneMessage>(new DmgDoneMessage() { Damage = new DmgModel() { Text = "333", Top = 48, FontSize = 24 } });

            Messenger.Default.Send<DmgTakenMessage>(new DmgTakenMessage() { Damage = new DmgModel() { Text = "111", Top = 0, FontSize = 24 } });
            Messenger.Default.Send<DmgTakenMessage>(new DmgTakenMessage() { Damage = new DmgModel() { Text = "222", Top = 24, FontSize = vFontSize } });
            Messenger.Default.Send<DmgTakenMessage>(new DmgTakenMessage() { Damage = new DmgModel() { Text = "333", Top = 48, FontSize = 24 } });
        }


        private RelayCommand fToggleWindows;
        public RelayCommand ToggleWindows
        {
            get
            {
                if (fToggleWindows == null)
                {
                    fToggleWindows = new RelayCommand(this.ToggleWindowPlacement);
                }
                return fToggleWindows;
            }
        }

        private void ToggleWindowPlacement()
        {
            fWindowsLocked = !fWindowsLocked;
            if (fWindowsLocked)
            {
                fWindowService.LockWindows();
                LockText = "Unlock Windows";
            }
            else
            {
                fWindowService.UnlockWindows();
                LockText = "Lock Windows";
            }
        }

        private string fOutputConsole;
        public string OutputConsole
        {
            get { return fOutputConsole; }
            set
            {
                if (value != fOutputConsole)
                {
                    fOutputConsole = value;
                    RaisePropertyChanged("OutputConsole");
                }
            }
        }

        private void RecieveMessage(GenericMessage pMessage)
        {
            AppendText(pMessage.Message);
        }

        private void AppendText(string pStringToAppend)
        {
            OutputConsole += pStringToAppend + Environment.NewLine;
        }
    }
}