using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Proggy.Controls;

namespace Proggy.Views
{
    public class AlertDialog : DialogWindow
    {
        public AlertDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
