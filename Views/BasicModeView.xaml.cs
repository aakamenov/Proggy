using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Proggy.Views
{
    public class BasicModeView : UserControl
    {
        public BasicModeView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
