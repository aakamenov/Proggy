using System;
using System.Collections.Generic;
using System.Text;
using Proggy.Core;
using Proggy.Infrastructure;

namespace Proggy.ViewModels
{
    public class AdvancedModeViewModel : ViewModelBase
    {
        public GlobalControlsViewModel GlobalControls => globalControls;
        private readonly GlobalControlsViewModel globalControls;

        public AdvancedModeViewModel()
        {
            globalControls = new GlobalControlsViewModel(new SinglePulseTrackBuilder(), MetronomeMode.Advanced);
        }
    }
}
