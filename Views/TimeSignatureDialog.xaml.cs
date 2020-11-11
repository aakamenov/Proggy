using Avalonia.Markup.Xaml;
using Proggy.Controls;

namespace Proggy.Views
{
    public class TimeSignatureDialog : BaseWindow
    {
        public TimeSignatureDialog() 
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
