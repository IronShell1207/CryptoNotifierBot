using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WindowsCryptoWidget.Helpers
{
    public class ConsoleOutputsHelper : ObservableObject
    {
        /// <summary>
        /// Статичный экземпляр объекта <see cref="OfferAnalyticsHelper"/>.
        /// </summary>
        public static ConsoleOutputsHelper Instance => _instance.Value;

        private static readonly Lazy<ConsoleOutputsHelper> _instance = new(() => new ConsoleOutputsHelper());
        private DispatcherTimer updateTimer { get; }

        public ConsoleOutputsHelper()
        {
            LogBuilder = new StringBuilder();
            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromSeconds(1);
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
            ConfigureConsoleOutput();
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            LogOutput = LogBuilder.ToString();
        }

        /// <inheritdoc cref="LogOutput"/>
        private string _logOutput;

        /// <summary>
        /// Вывод лога.
        /// </summary>
        public string LogOutput
        {
            get => _logOutput;
            set => SetProperty(ref _logOutput, value);
        }

        private StringBuilder LogBuilder { get; }

        public void ConfigureConsoleOutput()
        {
            TextWriter outputStream = new StringWriter(LogBuilder);

            Console.SetOut(outputStream);
            LogBuilder.Capacity = 1000;
        }
    }
}