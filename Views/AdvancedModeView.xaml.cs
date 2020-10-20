using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Proggy.Views
{
    public class AdvancedModeView : UserControl
    {
        public AdvancedModeView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
