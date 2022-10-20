using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CryptoElement.xaml
    /// </summary>
    public partial class CryptoElement : UserControl
    {
        public static double CurNameFontSize = 20;
        public static double PriceFontSize = 20;
        public static double ArrowProceSize = 16;
        public static double ProcentSize = 12;
        public static double PriceChangeSize = 9;

        public double Size
        {
            set
            {
                Lbl_CurrencyName.FontSize = CurNameFontSize * value;
                Lbl_PairValue.FontSize = PriceFontSize * value;
                lbl_ArrowPricing.FontSize = ArrowProceSize * value;
                Lbl_PairProc.FontSize = ProcentSize * value;
                Lbl_pricechange.FontSize = PriceChangeSize * value;
            }
        }

        public string PairName
        {
            get
            {
                return Lbl_CurrencyName.Text;
            }
            set
            {
                Lbl_CurrencyName.Text = value;
            }
        }

        public double Price
        {
            set
            {
                double lastprice = double.Parse(Lbl_PairValue.Text);
                Lbl_PairValue.Text = value.ToString();
                lbl_ArrowPricing.Text = lastprice < value ? "▲" : "▼";
                lbl_ArrowPricing.Foreground = lastprice < value ? Brushes.Green : Brushes.Red;
                if (value < 0.000001)
                    Lbl_PairValue.FontSize = Lbl_PairValue.FontSize * 0.75;
            }
        }

        private void PriceAnimation(TextBlock tb, double oldprice, double newprice)
        {
        }

        public double Procent
        {
            set
            {
                Lbl_PairProc.Text = value.ToString() + "%";
                if (value > 0.05)
                    Lbl_PairProc.Foreground = Brushes.Green;
                else if (value < 0.05)
                    Lbl_PairProc.Foreground = Brushes.Red;
                else
                    Lbl_PairProc.Foreground = Brushes.White;
            }
        }

        public double PriceChanges24h
        {
            set
            {
                Lbl_pricechange.Text = value.ToString();
                if (value > 0)
                    Lbl_pricechange.Foreground = Brushes.Green;
                else if (value < 0)
                    Lbl_pricechange.Foreground = Brushes.Red;
                else
                    Lbl_pricechange.Foreground = Brushes.White;
            }
        }

        private void Lbl_CurrencyName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Released)
            {
                var mainlin = "https://www.binance.com/ru/trade/";
                Regex usdtre = new Regex(@"(?<curname>[A-Z0-9]{2,6})USDT");
                Regex btctre = new Regex(@"(?<curname>[A-Z0-9]{2,6})BTC");
                var name = ((TextBlock)sender).Text;
                if (usdtre.IsMatch(name))
                {
                    mainlin += $"{usdtre.Replace(name, "${curname}")}_USDT";
                    goto openlink;
                }
                else if (btctre.IsMatch(name))
                {
                    mainlin += $"{btctre.Replace(name, "${curmame}")}_BTC";
                    goto openlink;
                }
                else
                {
                    return;
                }
            openlink:
                Process.Start("explorer", mainlin);
            }
        }

        public CryptoElement()
        {
            InitializeComponent();
        }
    }
}