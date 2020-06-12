using System;
using System.Globalization;
using System.Windows.Data;

namespace EQFCT.Converter
{
    public class EnabledDisabledToBooleanConverter : IValueConverter
    {
        private const string EnabledText = "Yes";
        private const string DisabledText = "No";
        public static readonly EnabledDisabledToBooleanConverter Instance = new EnabledDisabledToBooleanConverter();

        public EnabledDisabledToBooleanConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(true, value)
                ? EnabledText
                : DisabledText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Actually won't be used, but in case you need that
            return Equals(value, EnabledText);
        }
    }
}
