using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WindowsCryptoWidget.Models;

namespace WindowsCryptoWidget.Converters
{
    internal class WidgetStyleToDataStyleConverter : IValueConverter
    {
        public DataTemplate DefaultWidgetStyle { get; set; }
        public DataTemplate SimpleWidgetTemplate { get; set; }
        public DataTemplate InLineWidgetTemplate { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            WidgetStyle style = (WidgetStyle)value;
            return style.Index switch
            {
                0 => DefaultWidgetStyle,
                1 => SimpleWidgetTemplate,
                2 => InLineWidgetTemplate
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return WidgetStyles.DefaultWidgetStyle;
        }
    }
}