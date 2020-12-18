using Avalonia.Markup.Xaml;
using Proggy.Controls;

namespace Proggy.Views
{
    public class OpenClickTrackDialog : DialogWindow
    {
        public OpenClickTrackDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
