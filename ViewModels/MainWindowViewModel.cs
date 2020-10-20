using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using Proggy.Infrastructure.Events;
using Proggy.Infrastructure;

namespace Proggy.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ViewModelBase CurrentView 
        {
            get => currentView;
            set => this.RaiseAndSetIfChanged(ref currentView, value);
        }

        private ViewModelBase currentView;

        public MainWindowViewModel()
        {
            currentView = new BasicModeViewModel();
            MessageBus.Current.Listen<ModeChanged>().Subscribe(OnModeChanged);
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
