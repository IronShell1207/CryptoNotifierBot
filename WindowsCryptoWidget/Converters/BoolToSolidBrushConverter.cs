using System;
using System.Globalization;
using System.Windows.Data;

namespace WindowsCryptoWidget.Converters
{
    public class BoolToSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}