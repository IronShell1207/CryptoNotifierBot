using System.Collections.Generic;

namespace WindowsCryptoWidget.Models
{
    public class WidgetStyle
    {
        public int Index { get; set; } = 0;
        public double DataWidth { get; set; }
        public double DataHeight { get; set; }
        public string StyleResourceName { get; set; }
    }

    public static class WidgetStyles
    {
        public static WidgetStyle InLineWidgetTemplate = new WidgetStyle()
        {
            StyleResourceName = "InLineWidgetTemplate",
            DataWidth = 300,
            DataHeight = 48,
            Index = 2
        };

        public static WidgetStyle DefaultWidgetStyle = new WidgetStyle()
        {
            StyleResourceName = "DefaultWidgetStyle",
            DataWidth = 150,
            Index = 0,
            DataHeight = 90
        };

        public static WidgetStyle SimpleWidgetTemplate = new WidgetStyle()
        {
            StyleResourceName = "SimpleWidgetTemplate",
            DataWidth = 170,
            Index = 1,
            DataHeight = 72
        };

        public static List<WidgetStyle> AllStyles = new()
        {
            DefaultWidgetStyle, SimpleWidgetTemplate, InLineWidgetTemplate
        };
    }
}