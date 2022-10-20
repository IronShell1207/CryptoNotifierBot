using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsCryptoWidget.Controls
{
    /// <summary>
    /// Interaction logic for SettingsCryptoElementControl.xaml
    /// </summary>
    public partial class SettingsCryptoElementControl : UserControl
    {
        public SettingsCryptoElementControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler ButtonUP_click
        {
            add
            {
                ButtonUp.Click += value;
            }
            remove
            {
                ButtonUp.Click += value;
            }
        }

        public event RoutedEventHandler ButtonDown_click
        {
            add
            {
                ButtonDown.Click += value;
            }
            remove
            {
                ButtonDown.Click += value;
            }
        }

        public event RoutedEventHandler ButtonRemove_click
        {
            add
            {
                ButtonRemove.Click += value;
            }
            remove
            {
                ButtonRemove.Click += value;
            }
        }

        public string CurrName
        {
            get
            {
                return lblCurname.Text;
            }
            set
            {
                lblCurname.Text = value;
            }
        }
    }
}