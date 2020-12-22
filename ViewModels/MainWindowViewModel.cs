using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using ReactiveUI;
using Proggy.Infrastructure.Events;
using Proggy.Infrastructure;

namespace Proggy.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public short WindowWidth => currentView is BasicModeViewModel ? basicSize.Item1 : advancedSize.Item1;
        public short WindowHeight => currentView is BasicModeViewModel ? basicSize.Item2 : advancedSize.Item2;
        public bool CanResize => currentView is BasicModeViewModel ? false : true;
        public Action Shutdown { get; set; }

        public ViewModelBase CurrentView 
        {
            get => currentView;
            set => this.RaiseAndSetIfChanged(ref currentView, value);
        }

        private ViewModelBase currentView;
        private (short, short) basicSize = (350, 250);
        private (short, short) advancedSize = (750, 650);

        public MainWindowViewModel()
        {
            currentView = new BasicModeViewModel();
            MessageBus.Current.Listen<ModeChanged>().Subscribe(OnModeChanged);

            this.WhenAnyValue(x => x.CurrentView).Subscribe(_ => 
            {
                this.RaisePropertyChanged(nameof(WindowWidth));
                this.RaisePropertyChanged(nameof(WindowHeight));
                this.RaisePropertyChanged(nameof(CanResize));
            });
        }

        public override async void OnClosing()
        {
            await PromptSave();

            CloseCurrentView();

            Shutdown?.Invoke();
        }

        private async void OnModeChanged(ModeChanged msg)
        {
            await PromptSave();

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

        private async Task PromptSave()
        {
            if (CurrentView is AdvancedModeViewModel vm)
                await vm.PromptSave();
        }
    }
}
