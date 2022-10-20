using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CryptoApi.Constants;
using WindowsCryptoWidget.Controls;
using WindowsCryptoWidget.Helpers;
using WindowsCryptoWidget.Windows;

namespace WindowsCryptoWidget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool ThreadSuspendder = true;

        public MainWindow()
        {
            InitializeComponent();
            GenerateWidgetStartup();
            MainBackground.Opacity = SettingsHelpers.SettingsH.WOpacity;
            FormScaler.ScaleX = SettingsHelpers.SettingsH.WSize;
            FormScaler.ScaleY = SettingsHelpers.SettingsH.WSize;
        }

        private void GenerateWidgetStartup()
        {
            if (SettingsHelpers.SettingsH.FavPairs.Count > 0)
            {
                CurrenciesWigdet.Children.Clear();
                MainWin.Height = 50;
                foreach (string pair in SettingsHelpers.SettingsH.FavPairs)
                {
                    AddCur(pair);
                }
                CurrUpdater.Start();
            }
        }

        private CryptoElement elementFinder(string PairName)
        {
            if (CurrenciesWigdet.Children.Count > 0)
            {
                foreach (UIElement el in CurrenciesWigdet.Children)
                {
                    var element = (CryptoElement)el;
                    if (element.PairName == PairName)
                        return element;
                }
            }
            return null;
        }

        private Thread _curruptr;

        private Thread CurrUpdater
        {
            get
            {
                if (_curruptr != null) return _curruptr;
                _curruptr = new Thread(async () =>
                {
                    try
                    {
                        while (ThreadSuspendder)
                        {
                            if (SettingsHelpers.SettingsH.FavPairs.Count > 0 && ExchangesHelper.LocalDataRequester.DataAvailable)
                            {
                                foreach (string Pair in SettingsHelpers.SettingsH.FavPairs)
                                {
                                    CryptoElement elem = null;
                                    Dispatcher.Invoke(new Action(() => { elem = elementFinder(Pair); }));
                                    if (elem != null)
                                    {
                                        string pairName = Pair.Split("/").First();
                                        string quoteName = Pair.Split("/").Last();
                                        var objcCurNow = await
                                            ExchangesHelper.LocalDataRequester.GetKucoinData(pairName, quoteName);
                                        if (objcCurNow != null)
                                            Dispatcher.Invoke(new Action(() =>
                                            {
                                                elem.Price = double.Parse(objcCurNow.Price);
                                                elem.PriceChanges24h = double.Parse(objcCurNow.changePrice);
                                                elem.Procent = Math.Round(double.Parse(objcCurNow.changeRate) * 100, 3);
                                            }));
                                    }
                                    else
                                    {
                                        SettingsHelpers.SettingsH.FavPairs.Remove(Pair);
                                    }
                                }
                            }
                            Thread.Sleep(2700);
                        }
                    }
                    catch (TaskCanceledException)
                    {
                    }
                });
                return _curruptr;
            }
            set
            {
                _curruptr = value;
            }
        }

        private void AddCur(string pairname)
        {
            var cryptoW = new CryptoElement();
            cryptoW.PairName = pairname;

            CurrenciesWigdet.Children.Add(cryptoW);
            MainWin.Height += 100;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseButtonState.Pressed == e.LeftButton)
                DragMove();
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(CurrenciesWigdet.Children, FormScaler, MainBackground);
            ThreadSuspendder = false;
            CurrUpdater = null;
            sw.ShowDialog();
            ThreadSuspendder = true;
            GenerateWidgetStartup();
        }

        private double lastpos = 0;

        private void ButtonRestoreWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Left = lastpos;
            Ushko.Visibility = Visibility.Collapsed;
            this.Width = 140;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lastpos = this.Left;
            this.Left = -190;
            Ushko.Visibility = Visibility.Visible;
            this.Width = 240;
        }
    }
}