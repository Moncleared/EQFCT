using GalaSoft.MvvmLight;
using System.Windows.Media;

namespace EQFCT.Model
{
    public class DmgModel : ObservableObject
    {
        private int fTop;
        public int Top
        {
            get
            {
                return fTop;
            }
            set
            {
                fTop = value;
                RaisePropertyChanged("Top");
            }
        }

        private int fLeft;
        public int Left
        {
            get
            {
                return fLeft;
            }
            set
            {
                fLeft = value;
                RaisePropertyChanged("Left");
            }
        }

        private string fText;
        public string Text
        {
            get
            {
                return fText;
            }
            set
            {
                fText = value;
                RaisePropertyChanged("Text");
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
            }
        }

        private bool fIsCritical;
        public bool IsCritical
        {
            get
            {
                return fIsCritical;
            }
            set
            {
                fIsCritical = value;
                RaisePropertyChanged("IsCritical");
            }
        }

        private bool fIsMiss;
        public bool IsMiss
        {
            get
            {
                return fIsMiss;
            }
            set
            {
                fIsMiss = value;
                RaisePropertyChanged("IsMiss");
            }
        }
    }
}
