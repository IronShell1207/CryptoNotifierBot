using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WindowsCryptoWidget.Controls;
using WindowsCryptoWidget.Helpers;

namespace WindowsCryptoWidget.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private UIElementCollection uisS;
        private ScaleTransform Scaller;
        private Rectangle rectang;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Slide_Transperency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsHelpers.SettingsConfig.WOpacity = rectang.Opacity = ((Slider)sender).Value / 100;
            JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Slide_Transperency.ValueChanged += Slide_Transperency_ValueChanged;
            Slider_WidgetSize.ValueChanged += Slider_Scaler_ValueChanged;
            CreateCurlist();
        }

        private void CreateCurlist()
        {
            if (SettingsHelpers.SettingsConfig.FavPairs.Count > 0)
            {
                listView_currienses.Items.Clear();
                foreach (string pair in SettingsHelpers.SettingsConfig.FavPairs)
                {
                    var element = new SettingsCryptoElementControl { CurrName = pair };
                    element.ButtonRemove_click += ElementRemoveClick;
                    element.ButtonDown_click += ElementDownClick;
                    element.ButtonUP_click += ElementUPClick;
                    listView_currienses.Items.Add(element);
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string curname = TB_CurName.Text.Trim().ToUpper();
            if (!String.IsNullOrWhiteSpace(curname))
            {
                if (!SettingsHelpers.SettingsConfig.FavPairs.Contains(curname))
                {
                    SettingsHelpers.SettingsConfig.FavPairs.Add(curname);
                    JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
                    CreateCurlist();
                    TB_CurName.Text = "";
                    //ControlsAssists.Snackbarer(SnackbarTwo, 1.5, "Pair has been added!", this).Start();
                }
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            // ExchangesHelper.LocalDataRequester.UpdateDelay = TimeSpan.FromSeconds(Slider_UpdateDelay.Value);
            this.Close();
        }

        private void ElementRemoveClick(object sender, RoutedEventArgs e)
        {
            var Curname = ((SettingsCryptoElementControl)(((StackPanel)(((Button)sender).Parent)).Parent)).CurrName;
            SettingsHelpers.SettingsConfig.FavPairs.Remove(Curname);
            JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
            CreateCurlist();
            //ControlsAssists.Snackbarer(SnackbarTwo, 1.5, "Pair has been removed!", this).Start();
        }

        private void ElementDownClick(object sender, RoutedEventArgs e)
        {
            var Curname = ((SettingsCryptoElementControl)(((StackPanel)(((Button)sender).Parent)).Parent)).CurrName;

            var currentId = SettingsHelpers.SettingsConfig.FavPairs.FindIndex(x => x == Curname);
            if (currentId < SettingsHelpers.SettingsConfig.FavPairs.Count - 1)
            {
                SettingsHelpers.SettingsConfig.FavPairs.Remove(Curname);
                SettingsHelpers.SettingsConfig.FavPairs.Insert(currentId + 1, Curname);
                JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
                CreateCurlist();
            }
        }

        private void ElementUPClick(object sender, RoutedEventArgs e)
        {
            var Curname = ((SettingsCryptoElementControl)(((StackPanel)(((Button)sender).Parent)).Parent)).CurrName;

            var currentId = SettingsHelpers.SettingsConfig.FavPairs.FindIndex(x => x == Curname);
            if (currentId > 0)
            {
                SettingsHelpers.SettingsConfig.FavPairs.Remove(Curname);
                SettingsHelpers.SettingsConfig.FavPairs.Insert(currentId - 1, Curname);
                JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
                CreateCurlist();
            }
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        private void Slider_Scaler_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsHelpers.SettingsConfig.WSize = Scaller.ScaleX = ((Slider)sender).Value * 0.01;
            Scaller.ScaleY = ((Slider)sender).Value * 0.01;
            JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
        }

        private void TB_CurName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonAdd.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void SettingsWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseButtonState.Pressed == e.LeftButton)
                DragMove();
        }
    }
}