using System.ComponentModel;
using System.Windows.Input;
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

        private void OnAdvancedModeCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (Content is AdvancedModeViewModel vm)
            {
                if (vm.IsDialogOpen)
                    return;

                var command = (string)e.Parameter;

                switch(command)
                {
                    case nameof(AdvancedModeViewModel.NewCommand):
                        vm.NewCommand.Execute(null);
                        break;
                    case nameof(AdvancedModeViewModel.OpenCommand):
                        vm.OpenCommand.Execute(null);
                        break;
                    case nameof(AdvancedModeViewModel.SaveCommand):
                        vm.SaveCommand.Execute(null);
                        break;
                    case nameof(AdvancedModeViewModel.OnItemClickedCommand):
                        vm.OnItemClickedCommand.Execute(null);
                        break;
                    default:
                        break;
                }
            }          
        }
    }
}
