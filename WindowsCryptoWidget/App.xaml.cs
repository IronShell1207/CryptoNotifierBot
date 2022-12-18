using System.Runtime.InteropServices;
using System.Windows;

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