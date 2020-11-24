using System.Threading.Tasks;
using Proggy.Core;
using Proggy.Infrastructure;
using NAudio.Wave;

namespace Proggy.ViewModels
{
    public class BasicModeViewModel : ViewModelBase
    {
        public GlobalControlsViewModel GlobalControls => globalControls;
        public TimeSignatureControlsViewModel TimeSignatureControls => timeSignatureControls;

        private readonly GlobalControlsViewModel globalControls;
        private readonly TimeSignatureControlsViewModel timeSignatureControls;

        public BasicModeViewModel()
        {        
            globalControls = new GlobalControlsViewModel(BuildPulse, MetronomeMode.Basic);
            timeSignatureControls = new TimeSignatureControlsViewModel();
           
        }

        private Task<ISampleProvider> BuildPulse()
        {
            var pulse = ClickTrackBuilder.BuildSinglePulse(
                new BarInfo(TimeSignatureControls.TempoNumericBoxValue,
                            TimeSignatureControls.SelectedBeats,
                            TimeSignatureControls.SelectedNoteLength)
            );

            return Task.FromResult(pulse);
        }
    }
}
