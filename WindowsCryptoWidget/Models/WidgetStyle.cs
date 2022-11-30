namespace WindowsCryptoWidget.Models
{
    public class WidgetStyle
    {
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
            DataHeight = 48
        };
        public static WidgetStyle DefaultWidgetStyle = new WidgetStyle()
        {
            StyleResourceName = "DefaultWidgetStyle",
            DataWidth = 170,
            DataHeight = 84
        };
        public static WidgetStyle SimpleWidgetTemplate = new WidgetStyle()
        {
            StyleResourceName = "SimpleWidgetTemplate",
            DataWidth = 170,
            DataHeight = 50
        };
    }
}