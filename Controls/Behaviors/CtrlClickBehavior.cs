using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using Avalonia.Controls;
using Proggy.Infrastructure;

namespace Proggy.Controls.Behaviors
{
    class CtrlClickBehavior : Trigger
    {
        public static readonly StyledProperty<Button> ButtonProperty =
            AvaloniaProperty.Register<CtrlClickBehavior, Button>(nameof(Button));

        public Button Button
        {
            get => GetValue(ButtonProperty);
            set => SetValue(ButtonProperty, value);
        }

        protected override void OnAttached()
        {
            if (Button is null)
                return;

            Button.Click += OnButtonClick;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (WindowNavigation.CurrentWindowInput.IsCtrlPressed)
            {
                e.Handled = true; //Override the click event
                Interaction.ExecuteActions(Button, Actions, e);
            }
        }
    }
}
