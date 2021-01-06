using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Proggy.Controls
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(int),
                typeof(NumericUpDown),
                new FrameworkPropertyMetadata(OnValuePropertyChanged) 
                {
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register(
                nameof(Min),
                typeof(int),
                typeof(NumericUpDown),
                new UIPropertyMetadata(0));

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(
                nameof(Max),
                typeof(int),
                typeof(NumericUpDown),
                new UIPropertyMetadata(100, OnMaxPropertyChanged));

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public int Min
        {
            get => (int)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public int Max
        {
            get => (int)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public NumericUpDown()
        {
            InitializeComponent();

            TextBox.PreviewTextInput += OnTextBoxPreviewInput;
            TextBox.LostFocus += OnTextBoxLostFocus;
            TextBox.TextChanged += OnTextBoxTextChanged;
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumericUpDown control)
                control.UpdateText();
        }

        private static void OnMaxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumericUpDown control)
                control.TextBox.MaxLength = e.NewValue.ToString().Length;
        }

        private void UpdateText()
        {
            TextBox.Text = Value.ToString();
        }

        private void OnTextBoxPreviewInput(object sender, TextCompositionEventArgs e)
        {
            if(e.Text.Length >= Max.ToString().Length)
            {
                e.Handled = true;
                return;
            }

            foreach (var c in e.Text)
            {
                if (char.IsLetter(c) || char.IsWhiteSpace(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }
        
        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(TextBox.Text, out int value))
                return;

            Value = value;
        }
        
        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            Value = Math.Clamp(Value, Min, Max);
            TextBox.Text = Value.ToString();
        }

        private void OnUpButtonClick(object sender, RoutedEventArgs e)
        {
            if (Value < Max)
                Value++;
        }

        private void OnDownButtonClick(object sender, RoutedEventArgs e)
        {
            if(Value > Min)
                Value--;
        }
    }
}
