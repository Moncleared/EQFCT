using EQFCT.Model;
using EQFCT.View;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EQFCT.Service
{
    /// <summary>
    /// Abstracting Window Creation/Handling from ViewModels
    /// </summary>
    public class WindowService
    {
        public DmgTakenWnd fDmgTakenWnd;
        public DmgDoneWnd fDmgDoneWnd;

        private bool fWindowsUnlocked = false;

        /// <summary>
        /// Creates the DmgTaken & DmgDone windows
        /// </summary>
        public void InitializeWindows()
        {
            fDmgTakenWnd = new DmgTakenWnd();
            fDmgTakenWnd.WindowStartupLocation = WindowStartupLocation.Manual;
            fDmgTakenWnd.Left = Properties.Settings.Default.DmgTakenLeft;
            fDmgTakenWnd.Top = Properties.Settings.Default.DmgTakenTop;
            fDmgTakenWnd.Height = Properties.Settings.Default.DmgTakenHeight;
            fDmgTakenWnd.Show();

            fDmgDoneWnd = new DmgDoneWnd();
            fDmgDoneWnd.WindowStartupLocation = WindowStartupLocation.Manual;
            fDmgDoneWnd.Left = Properties.Settings.Default.DmgDoneLeft;
            fDmgDoneWnd.Top = Properties.Settings.Default.DmgDoneTop;
            fDmgDoneWnd.Height = Properties.Settings.Default.DmgDoneHeight;
            fDmgDoneWnd.Show();

            CreateUnlockedWindowThread();
        }

        /// <summary>
        /// Creates a background thread that will just send text to the DmgTaken and DmgDone windows during "Unlock" mode
        /// </summary>
        private void CreateUnlockedWindowThread()
        {
            Task.Run(() =>
            {
                Random vRandom = new Random();
                while (true)
                {
                    Thread.Sleep(500);
                    if (fWindowsUnlocked)
                    {
                        Messenger.Default.Send<DmgDoneMessage>(new DmgDoneMessage() { Damage = new DmgModel() { Text = String.Format("{0:n0}", vRandom.Next(1, 10000)), Top = 0, FontSize = 24 } });
                        Messenger.Default.Send<DmgDoneMessage>(new DmgDoneMessage() { Damage = new DmgModel() { Text = String.Format("{0:n0}", vRandom.Next(1, 10000)), IsCritical = true, Top = 0, FontSize = 24 } });
                        Messenger.Default.Send<DmgDoneMessage>(new DmgDoneMessage() { Damage = new DmgModel() { Text = String.Format("{0:n0}", vRandom.Next(1, 10000)), IsHeal = true, Top = 0, FontSize = 24 } });

                        Messenger.Default.Send<DmgTakenMessage>(new DmgTakenMessage() { Damage = new DmgModel() { Text = String.Format("-{0:n0}", vRandom.Next(1, 10000)), Top = 0, FontSize = 24 } });
                        Messenger.Default.Send<DmgTakenMessage>(new DmgTakenMessage() { Damage = new DmgModel() { Text = String.Format("-{0:n0}", vRandom.Next(1, 10000)), IsCritical = true, Top = 0, FontSize = 24 } });
                        Messenger.Default.Send<DmgTakenMessage>(new DmgTakenMessage() { Damage = new DmgModel() { Text = String.Format("+{0:n0}", vRandom.Next(1, 10000)), IsHeal = true, Top = 0, FontSize = 24 } });
                    }
                }
            });
        }

        /// <summary>
        /// Unlocks both DmgDone and DmgTaken window
        /// </summary>
        public void UnlockWindows()
        {
            fDmgDoneWnd.Close();
            fDmgDoneWnd = new DmgDoneWnd();
            fDmgDoneWnd.WindowStartupLocation = WindowStartupLocation.Manual;
            fDmgDoneWnd.Left = Properties.Settings.Default.DmgDoneLeft;
            fDmgDoneWnd.Top = Properties.Settings.Default.DmgDoneTop;
            fDmgDoneWnd.Height = Properties.Settings.Default.DmgDoneHeight;
            fDmgDoneWnd.AllowsTransparency = false;
            fDmgDoneWnd.WindowStyle = WindowStyle.ToolWindow;
            fDmgDoneWnd.Background = Brushes.Black;
            fDmgDoneWnd.Show();

            fDmgTakenWnd.Close();
            fDmgTakenWnd = new DmgTakenWnd();
            fDmgTakenWnd.WindowStartupLocation = WindowStartupLocation.Manual;
            fDmgTakenWnd.Left = Properties.Settings.Default.DmgTakenLeft;
            fDmgTakenWnd.Top = Properties.Settings.Default.DmgTakenTop;
            fDmgTakenWnd.Height = Properties.Settings.Default.DmgTakenHeight;
            fDmgTakenWnd.AllowsTransparency = false;
            fDmgTakenWnd.WindowStyle = WindowStyle.ToolWindow;
            fDmgDoneWnd.Background = Brushes.Black;
            fDmgTakenWnd.Show();
            fWindowsUnlocked = true;
        }

        /// <summary>
        /// Locking the DmgDone and DmgTaken windows while storing off their locations and sizes in settings
        /// </summary>
        public void LockWindows()
        {
            //Lock DmgDoneWindow
            Properties.Settings.Default.DmgDoneTop = fDmgDoneWnd.Top;
            Properties.Settings.Default.DmgDoneLeft = fDmgDoneWnd.Left;
            Properties.Settings.Default.DmgDoneHeight = fDmgDoneWnd.Height;
            Properties.Settings.Default.Save();
            Messenger.Default.Send<DmgDoneHeightChg>(new DmgDoneHeightChg { Height = fDmgDoneWnd.Height });
            fDmgDoneWnd.Close();
            fDmgDoneWnd = new DmgDoneWnd();
            fDmgDoneWnd.WindowStartupLocation = WindowStartupLocation.Manual;
            fDmgDoneWnd.Left = Properties.Settings.Default.DmgDoneLeft;
            fDmgDoneWnd.Top = Properties.Settings.Default.DmgDoneTop;
            fDmgDoneWnd.Height = Properties.Settings.Default.DmgDoneHeight;
            fDmgDoneWnd.AllowsTransparency = true;
            fDmgDoneWnd.WindowStyle = WindowStyle.None;
            fDmgDoneWnd.Background = Brushes.Transparent;
            fDmgDoneWnd.Show();

            //Lock DmgTakenWindow
            Properties.Settings.Default.DmgTakenTop = fDmgTakenWnd.Top;
            Properties.Settings.Default.DmgTakenLeft = fDmgTakenWnd.Left;
            Properties.Settings.Default.DmgTakenHeight = fDmgTakenWnd.Height;
            Properties.Settings.Default.Save();
            Messenger.Default.Send<DmgTakenHeightChg>(new DmgTakenHeightChg { Height = fDmgTakenWnd.Height });
            fDmgTakenWnd.Close();
            fDmgTakenWnd = new DmgTakenWnd();
            fDmgTakenWnd.WindowStartupLocation = WindowStartupLocation.Manual;
            fDmgTakenWnd.Left = Properties.Settings.Default.DmgTakenLeft;
            fDmgTakenWnd.Top = Properties.Settings.Default.DmgTakenTop;
            fDmgTakenWnd.Height = Properties.Settings.Default.DmgTakenHeight;
            fDmgTakenWnd.AllowsTransparency = true;
            fDmgTakenWnd.WindowStyle = WindowStyle.None;
            fDmgTakenWnd.Background = Brushes.Transparent;
            fDmgTakenWnd.Show();
            fWindowsUnlocked = false;
        }
    }
}
