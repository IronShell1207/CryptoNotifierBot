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
    /// Interaction logic for SliderControl.xaml
    /// </summary>
    public partial class SliderControl : UserControl
    {
        /// <inheritdoc cref="Value"/>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register
            (
                nameof(Value),
                typeof(double),
                typeof(SliderControl),
                new PropertyMetadata(default(double))
            );

        /// <summary>
        /// Значение.
        /// </summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double MinValue
        {
            get
            {
                return Slider.Minimum;
            }
            set
            {
                Slider.Minimum = value;
            }
        }

        public double MaxValue
        {
            get
            {
                return Slider.Maximum;
            }
            set
            {
                Slider.Maximum = value;
            }
        }

        public string MinText
        {
            get
            {
                return LeftText.Text;
            }
            set
            {
                LeftText.Text = value;
            }
        }

        public string MaxText
        {
            get
            {
                return RightText.Text;
            }
            set
            {
                RightText.Text = value;
            }
        }

        public string TitleText
        {
            get
            {
                return TitleTextBox.Text;
            }
            set
            {
                TitleTextBox.Text = value;
            }
        }

        public double StepVal
        {
            get
            {
                return Slider.TickFrequency;
            }
            set
            {
                Slider.TickFrequency = value;
            }
        }

        public SliderControl()
        {
            InitializeComponent();
        }
    }
}