using System;
using System.Globalization;
using System.Windows.Data;

namespace WindowsCryptoWidget.Converters
{
    internal class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubledValue = (double)value;
            if (doubledValue <= 0.0)
            {
                return "not available";
            }

            return doubledValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}