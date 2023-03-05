using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace WindowsCryptoWidget
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //NativeMethods.AllocConsole();
            AppCenter.Start("6bc45cfa-c83c-4b08-a1bb-fb66a5ff9e87",
                typeof(Analytics), typeof(Crashes));
            base.OnStartup(e);
        }
    }

    /// <summary>
    /// Нативные методы.
    /// </summary>
    internal static class NativeMethods
    {
        #region Public Methods

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        #endregion Public Methods
    }
}