using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Proggy.Controls
{
    /// <summary>
    /// Interaction logic for EditableTextBlock.xaml
    /// </summary>
    public partial class EditableTextBlock : UserControl
    {
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(EditableTextBlock),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private string oldTextValue;

        public EditableTextBlock()
        {
            InitializeComponent();
            HideTextBox();
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                HideTextBox();
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            HideTextBox();
        }

        private void OnTextBlockMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowTextBox();
        }

        private void ShowTextBox()
        {
            TextBlock.Visibility = Visibility.Collapsed;

            TextBox.Visibility = Visibility.Visible;
            TextBox.CaretIndex = TextBox.Text.Length;
            TextBox.Focus();

            oldTextValue = Text;
        }

        private void HideTextBox()
        {
            TextBlock.Visibility = Visibility.Visible;
            TextBox.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(Text))
                Text = oldTextValue;
        }
    }
}
