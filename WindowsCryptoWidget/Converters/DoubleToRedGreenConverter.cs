using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WindowsCryptoWidget.Converters
{
    internal class DoubleToRedGreenConverter : IValueConverter
    {
        private SolidColorBrush RedColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        private SolidColorBrush GreenColor = new SolidColorBrush(Color.FromArgb(255, 0, 220, 0));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? dval = (double?)value;
            if (dval == null) return RedColor;
            return dval > 0 ? GreenColor : RedColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0.00;
        }
    }
}