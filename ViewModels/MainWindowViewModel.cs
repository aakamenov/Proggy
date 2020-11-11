using System;
using ReactiveUI;
using Proggy.Infrastructure.Events;
using Proggy.Infrastructure;
using System.Reactive.Linq;

namespace Proggy.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public short WindowWidth => currentView is BasicModeViewModel ? basicSize.Item1 : advancedSize.Item1;
        public short WindowHeight => currentView is BasicModeViewModel ? basicSize.Item2 : advancedSize.Item2;
        public bool CanResize => currentView is BasicModeViewModel ? false : true;

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

        private void OnModeChanged(ModeChanged msg)
        {
            if (msg.Mode == MetronomeMode.Basic)
                CurrentView = new BasicModeViewModel();
            else if (msg.Mode == MetronomeMode.Advanced)
                CurrentView = new AdvancedModeViewModel();
        }
    }
}
