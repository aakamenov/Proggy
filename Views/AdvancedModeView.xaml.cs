using System.Windows;
using System.Windows.Controls;
using Proggy.ViewModels;

namespace Proggy.Views
{
    /// <summary>
    /// Interaction logic for AdvancedModeView.xaml
    /// </summary>
    public partial class AdvancedModeView : Page
    {
        public AdvancedModeView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is AdvancedModeViewModel vm)
                vm.ScrollToBar = ScrollTo;
        }

        private void ScrollTo(int childIndex)
        {
            var child = BarItems.ItemContainerGenerator.ContainerFromIndex(childIndex);

            if (child is FrameworkElement element)
                element.BringIntoView();
        }
    }
}
