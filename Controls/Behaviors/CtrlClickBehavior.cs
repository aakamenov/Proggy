using System;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using Avalonia.Controls;
using Avalonia.Input;

namespace Proggy.Controls.Behaviors
{
    class CtrlClickBehavior : Trigger
    {
        public static readonly StyledProperty<Button> ButtonProperty =
            AvaloniaProperty.Register<CtrlClickBehavior, Button>(nameof(Button));

        private bool ctrlPressed;

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
            Button.KeyDown += OnButtonKeyDown;
            Button.KeyUp += OnButtonKeyUp;
        }

        private void OnButtonKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                ctrlPressed = false;
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                ctrlPressed = true;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (ctrlPressed)
            {
                e.Handled = true; //Override the click event
                Interaction.ExecuteActions(Button, Actions, e);
            }
        }
    }
}
