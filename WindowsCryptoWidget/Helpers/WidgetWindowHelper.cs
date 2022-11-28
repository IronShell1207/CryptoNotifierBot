using System;
using System.Windows;

namespace WindowsCryptoWidget.Helpers
{
    internal class WidgetWindowHelper
    {
        /// <summary>
        /// Статичный экземпляр класса <see cref="WidgetWindowHelper"/>.
        /// </summary>
        private static readonly Lazy<WidgetWindowHelper> _instance = new((() => new WidgetWindowHelper()));

        /// <summary>
        /// Статичный экземпляр класса <see cref="WidgetWindowHelper"/>.
        /// </summary>
        public static WidgetWindowHelper Instance => _instance.Value;

        public Window WidgetWindow { get; set; }

        public void Initialize(Window window)
        {
            WidgetWindow = window;
        }
    }
}