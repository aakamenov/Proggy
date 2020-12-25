using Avalonia.Controls;
using Avalonia.Input;

namespace Proggy.Infrastructure
{
    public class WindowInput
    {
        public bool IsCtrlPressed { get; private set; }

        public WindowInput(Window window)
        {
            window.KeyDown += OnWindowKeyDown;
            window.KeyUp += OnWindowKeyUp;
        }

        private void OnWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                IsCtrlPressed = false;
        }

        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                IsCtrlPressed = true;
        }
    }
}
