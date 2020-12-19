using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Proggy.ViewModels;

namespace Proggy.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (DataContext is ViewModelBase vm)
                vm.OnClosing();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
