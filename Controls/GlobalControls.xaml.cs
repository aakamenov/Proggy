using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Proggy.Controls
{
    public class GlobalControls : UserControl
    {
        public GlobalControls()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
