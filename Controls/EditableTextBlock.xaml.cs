using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Data;

namespace Proggy.Controls
{
    public class EditableTextBlock : UserControl
    {
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<EditableTextBlock, string>(nameof(Text), defaultBindingMode: BindingMode.TwoWay);

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private readonly TextBlock text;
        private readonly TextBox textBox;

        private string oldTextValue;

        public EditableTextBlock()
        {
            InitializeComponent();

            text = this.FindControl<TextBlock>("TextBlock");
            textBox = this.FindControl<TextBox>("TextBox");

            text.PointerPressed += OnTextPointerPressed;
            textBox.LostFocus += OnTextBoxLostFocus;
            textBox.KeyDown += OnTextBoxKeyDown;
            
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

        private void OnTextPointerPressed(object sender, PointerPressedEventArgs e)
        {
            ShowTextBox();
        }

        private void ShowTextBox()
        {
            text.IsVisible = false;

            textBox.IsVisible = true;
            textBox.CaretIndex = textBox.Text.Length;
            textBox.Focus();
            
            oldTextValue = Text;
        }

        private void HideTextBox()
        {
            text.IsVisible = true;
            textBox.IsVisible = false;

            if (string.IsNullOrWhiteSpace(Text))
                Text = oldTextValue;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
