using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Proggy.ViewModels;

namespace Proggy.Views
{
    public class MainWindow : Window
    {
        //This is to allow the VM to prompt the user before closing
        //by cancelling the closing event and letting the VM to call the Close() function
        private bool shutdown;

        public MainWindow()
        {
            InitializeComponent();

            var vm = new MainWindowViewModel()
            {
                Shutdown = () => 
                {
                    shutdown = true;
                    Close();
                }
            };

            DataContext = vm;

            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (DataContext is ViewModelBase vm)
            {
                if (!shutdown)
                {
                    e.Cancel = true;
                    vm.OnClosing();
                }
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
