using CryptoMonWidgets.Enums.WindowHelper;
using GlobalStructures;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CryptoMonWidgets.View.Windows.Base
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BorderlessWindow : Window
    {
        #region Public Constructors

        public BorderlessWindow()
        {
            this.InitializeComponent();

            Initialize();
        }

        #endregion Public Constructors

        #region Private Methods

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            Clean();
        }

       

        #endregion Private Methods
    }
}