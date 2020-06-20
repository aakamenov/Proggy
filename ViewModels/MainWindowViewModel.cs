using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

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
        }
    }
}
