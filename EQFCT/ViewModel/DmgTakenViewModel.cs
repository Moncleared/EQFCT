using EQFCT.Helper;
using EQFCT.Model;
using EQFCT.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
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
    public class DmgTakenViewModel : ViewModelBase
    {
        private ObservableCollection<DmgModel> fItemsToShowInCanvas;
        private BackgroundWorker fBackgroundWorker;

        public ObservableCollection<DmgModel> ItemsToShowInCanvas
        {
            get
            {
                return fItemsToShowInCanvas;
            }
            set
            {
                fItemsToShowInCanvas = value;
                RaisePropertyChanged("ItemsToShowInCanvas");
            }
        }

        private double fHeight;
        public double Height
        {
            get
            {
                return fHeight;
            }
            set
            {
                fHeight = value;
                RaisePropertyChanged("Height");
            }
        }

        private int fFontSize;
        public int FontSize
        {
            get
            {
                return fFontSize;
            }
            set
            {
                fFontSize = value;
                RaisePropertyChanged("FontSize");
                Properties.Settings.Default.DmgTakenFontSize = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color fFontColor;
        public Color FontColor
        {
            get
            {
                return fFontColor;
            }
            set
            {
                fFontColor = value;
                RaisePropertyChanged("FontColor");
                Properties.Settings.Default.DmgTakenFontColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color fHealFontColor;
        public Color HealFontColor
        {
            get
            {
                return fHealFontColor;
            }
            set
            {
                fHealFontColor = value;
                RaisePropertyChanged("HealFontColor");
                Properties.Settings.Default.HealFontColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool fShowMisses;
        public bool ShowMisses
        {
            get
            {
                return fShowMisses;
            }
            set
            {
                fShowMisses = value;
                RaisePropertyChanged("ShowMisses");
                Properties.Settings.Default.DmgTakenShowMisses = value;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public DmgTakenViewModel()
        {
            Messenger.Default.Register<DmgTakenMessage>(
                this, (action) => DamageTakenMessage(action)
                );
            Messenger.Default.Register<DmgTakenHeightChg>(
                this, (action) => UpdateWindowHeight(action)
                );
            Height = Properties.Settings.Default.DmgTakenHeight;
            FontSize = Properties.Settings.Default.DmgTakenFontSize;
            FontColor = Properties.Settings.Default.DmgTakenFontColor;
            ShowMisses = Properties.Settings.Default.DmgTakenShowMisses;
            HealFontColor = Properties.Settings.Default.HealFontColor;
            ItemsToShowInCanvas = new ObservableCollection<DmgModel>();

            fBackgroundWorker = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true
            };
            fBackgroundWorker.DoWork += floatText;
            fBackgroundWorker.RunWorkerAsync();
        }

        private void UpdateWindowHeight(DmgTakenHeightChg action)
        {
            Height = action.Height;
        }

        private void DamageTakenMessage(DmgTakenMessage pMessage)
        {            
            //Process the incoming damage model before drawing it
            pMessage.Damage.FontSize = this.FontSize;
            pMessage.Damage.FontColor = this.FontColor;

            if (pMessage.Damage.IsCritical) pMessage.Damage.FontSize = Convert.ToInt32(this.FontSize * 1.5);

            bool vShowMessage = true;

            //Only show misses if they're enabled
            if (pMessage.Damage.IsMiss && !this.ShowMisses) vShowMessage = false;

            if (pMessage.Damage.IsHeal) pMessage.Damage.FontColor = this.HealFontColor;

            if ( vShowMessage ) this.AddDmgModel(pMessage.Damage);
        }

        public void AddDmgModel(DmgModel pModel)
        {
            App.Current.Dispatcher.Invoke((Action)delegate {
                this.ItemsToShowInCanvas.Add(pModel);
            });
        }

        public void RemoveDmgModel(DmgModel pModel)
        {
            App.Current.Dispatcher.Invoke((Action)delegate {
                this.ItemsToShowInCanvas.Remove(pModel);
            });
        }

        private void floatText(object sender, DoWorkEventArgs e)
        {
            while ( true )
            {
                Thread.Sleep(10);

                //Space out all text evenly
                DrawHelper.PreventTextOverLap(this.fItemsToShowInCanvas);

                for (int i=this.fItemsToShowInCanvas.Count-1; i>=0; i--)
                {
                    if (this.fItemsToShowInCanvas[i].Top > this.Height)
                    {
                        this.RemoveDmgModel(this.fItemsToShowInCanvas[i]);
                    } else
                    {
                        this.fItemsToShowInCanvas[i].Top += 2;
                    }
                }
            }
        }
    }
}