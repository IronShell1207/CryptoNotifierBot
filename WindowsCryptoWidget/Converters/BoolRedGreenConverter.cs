using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WindowsCryptoWidget.Converters
{
    internal class BoolRedGreenConverter : IValueConverter
    {
        private SolidColorBrush RedColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        private SolidColorBrush GreenColor = new SolidColorBrush(Color.FromArgb(255, 0, 220, 0));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? GreenColor : RedColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SolidColorBrush)value == GreenColor;
        }
    }
}