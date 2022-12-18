using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WindowsCryptoWidget.Converters
{
    internal class PriceFontSizeConverter : IValueConverter
    {
        public double DefaultFontSize { get; set; } = 18;
        public double LowedFontSize { get; set; } = 14;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubledValue = (double)value;
            if (doubledValue <= 0.0)
            {
                return LowedFontSize;
            }

            return DefaultFontSize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}