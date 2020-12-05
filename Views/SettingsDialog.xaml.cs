using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Proggy.Views
{
    public class SettingsDialog : Window
    {
        public SettingsDialog()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
