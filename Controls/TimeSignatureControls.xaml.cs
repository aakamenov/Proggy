using System.Windows;
using System.Windows.Controls;

namespace Proggy.Controls
{
    /// <summary>
    /// Interaction logic for TimeSignatureControls.xaml
    /// </summary>
    public partial class TimeSignatureControls : UserControl
    {
        public static readonly DependencyProperty ShowTempoBoxProperty =
            DependencyProperty.Register(
                nameof(ShowTempoBox),
                typeof(bool),
                typeof(TimeSignatureControls),
                new UIPropertyMetadata(true));

        public bool ShowTempoBox
        {
            get => (bool)GetValue(ShowTempoBoxProperty);
            set => SetValue(ShowTempoBoxProperty, value);
        }

        public TimeSignatureControls()
        {
            InitializeComponent();
        }
    }
}
