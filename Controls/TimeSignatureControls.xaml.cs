using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Proggy.Controls
{
    public class TimeSignatureControls : UserControl
    {
        public static readonly DirectProperty<TimeSignatureControls, bool> ShowTempoBoxProperty =
            AvaloniaProperty.RegisterDirect<TimeSignatureControls, bool>(
                nameof(ShowTempoBox),
                (x) => x.ShowTempoBox,
                (x, y) => x.ShowTempoBox = y,
                true);

        public bool ShowTempoBox
        {
            get => showTempoBox;
            set => SetAndRaise(ShowTempoBoxProperty, ref showTempoBox, value);
        }

        private bool showTempoBox = true;

        public TimeSignatureControls()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
