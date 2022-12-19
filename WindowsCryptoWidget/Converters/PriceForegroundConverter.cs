using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WindowsCryptoWidget.Converters
{
    internal class PriceForegroundConverter : IValueConverter
    {
        public SolidColorBrush DefaultColor { get; set; } = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        public SolidColorBrush DisabledColor { get; set; } = new SolidColorBrush(Color.FromRgb(188, 0, 0));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubledValue = (double)value;
            if (doubledValue <= 0.0)
            {
                return DisabledColor;
            }

            return DefaultColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}