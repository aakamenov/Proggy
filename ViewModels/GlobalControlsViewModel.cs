using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using ReactiveUI;

namespace Proggy.ViewModels
{
    public class GlobalControlsViewModel : ViewModelBase
    {
        private readonly string[] modes = { "Basic", "Advanced" };

        public string[] Modes => modes;
        public string SelectedMode
        {
            get => selectedMode;
            set => this.RaiseAndSetIfChanged(ref selectedMode, value);
        }

        private string selectedMode;

        public GlobalControlsViewModel()
        {
            SelectedMode = modes[0];
        }
    }
}
