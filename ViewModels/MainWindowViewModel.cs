using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using Proggy.Infrastructure.Events;

namespace Proggy.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public double WindowWidth => currentView is BasicModeViewModel ? basicSize.Item1 : advancedSize.Item1;
        public double WindowHeight => currentView is BasicModeViewModel ? basicSize.Item2 : advancedSize.Item2;
        public ResizeMode ResizeMode => currentView is BasicModeViewModel ? ResizeMode.CanMinimize : ResizeMode.CanResize;

        public ViewModelBase CurrentView 
        {
            get => currentView;
            set => this.RaiseAndSetIfChanged(ref currentView, value);
        }

        private ViewModelBase currentView;

        private readonly (double, double) basicSize = (350, 250);
        private readonly (double, double) advancedSize = (780, 723);

        public MainWindowViewModel()
        {
            currentView = new BasicModeViewModel();
            MessageBus.Current.Listen<ModeChanged>().Subscribe(OnModeChanged);

            this.WhenAnyValue(x => x.CurrentView).Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(WindowWidth));
                this.RaisePropertyChanged(nameof(WindowHeight));
                this.RaisePropertyChanged(nameof(ResizeMode));
            });
        }

        public override async void OnClosing()
        {
            await PromptSave(true);

            CloseCurrentView();
        }
        
        private async void OnModeChanged(ModeChanged msg)
        {
            await PromptSave(false);

            CloseCurrentView();

            if (msg.Mode == MetronomeMode.Basic)
                CurrentView = new BasicModeViewModel();
            else if (msg.Mode == MetronomeMode.Advanced)
                CurrentView = new AdvancedModeViewModel();
        }
        
        private void CloseCurrentView()
        {
            if (CurrentView != null)
                CurrentView.OnClosing();
        }
        
        private async Task PromptSave(bool exiting)
        {
            if (CurrentView is AdvancedModeViewModel vm)
                await vm.PromptSave();

            if (exiting)
                Application.Current.Shutdown();
        }
    }
}
