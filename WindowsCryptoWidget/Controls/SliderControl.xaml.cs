using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

        /// <inheritdoc cref="Minimum"/>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register
            (
                nameof(Minimum),
                typeof(double),
                typeof(SliderControl),
                new PropertyMetadata(default(double))
            );

        /// <summary>
        /// Минимальное значение.
        /// </summary>
        public double Minimum
        {
            get => (double) GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        /// <inheritdoc cref="Maximum"/>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register
            (
                nameof(Maximum),
                typeof(double),
                typeof(SliderControl),
                new PropertyMetadata(default(double))
            );

        /// <summary>
        /// Максимальное значение.
        /// </summary>
        public double Maximum
        {
            get => (double) GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        /// <inheritdoc cref="Title"/>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register
            (
                nameof(Title),
                typeof(string),
                typeof(SliderControl),
                new PropertyMetadata(default(string))
            );

        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <inheritdoc cref="ValueTypeName"/>
        public static readonly DependencyProperty ValueTypeNameProperty =
            DependencyProperty.Register
            (
                nameof(ValueTypeName),
                typeof(string),
                typeof(SliderControl),
                new PropertyMetadata(default(string))
            );

        /// <summary>
        /// Название величины.
        /// </summary>
        public string ValueTypeName
        {
            get => (string) GetValue(ValueTypeNameProperty);
            set => SetValue(ValueTypeNameProperty, value);
        }

        /// <inheritdoc cref="IsCurrentValueDisplayed"/>
        public static readonly DependencyProperty IsCurrentValueDisplayedProperty =
            DependencyProperty.Register
            (
                nameof(IsCurrentValueDisplayed),
                typeof(bool),
                typeof(SliderControl),
                new PropertyMetadata(default(bool))
            );

        /// <summary>
        /// Отображается ли текущее значение в заголовке.
        /// </summary>
        public bool IsCurrentValueDisplayed
        {
            get => (bool) GetValue(IsCurrentValueDisplayedProperty);
            set => SetValue(IsCurrentValueDisplayedProperty, value);
        }

        /// <inheritdoc cref="Interval"/>
        public static readonly DependencyProperty IntervaProperty =
            DependencyProperty.Register
            (
                nameof(Interval),
                typeof(double),
                typeof(SliderControl),
                new PropertyMetadata(default(double))
            );

        /// <summary>
        /// Интервал
        /// </summary>
        public double Interval
        {
            get => (double) GetValue(IntervaProperty);
            set => SetValue(IntervaProperty, value);
        }

        /// <inheritdoc cref="DisplayedMinumum"/>
        public static readonly DependencyProperty DisplayedMinumumProperty =
            DependencyProperty.Register
            (
                nameof(DisplayedMinumum),
                typeof(string),
                typeof(SliderControl),
                new PropertyMetadata(default(string))
            );

        /// <summary>
        /// Отображаемое минимальное значение.
        /// </summary>
        public string DisplayedMinumum
        {
            get => (string) GetValue(DisplayedMinumumProperty) ??  Minimum.ToString();
            set => SetValue(DisplayedMinumumProperty, value);
        }


        public SliderControl()
        {
            InitializeComponent();
        }
    }
}