using System.ComponentModel;
using System.Windows;
using Proggy.ViewModels;

namespace Proggy.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Closing += OnClosing;

            DataContext = new MainWindowViewModel();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (DataContext is ViewModelBase vm)
                vm.OnClosing();
        }
    }
}
