using System.Threading.Tasks;
using Proggy.Core;
using Proggy.Models;
using Proggy.Infrastructure.Events;
using NAudio.Wave;

namespace Proggy.ViewModels
{
    public class BasicModeViewModel : ViewModelBase
    {
        public GlobalControlsViewModel GlobalControls { get; }
        public TimeSignatureControlsViewModel TimeSignatureControls { get; }

        public BasicModeViewModel()
        {
            GlobalControls = new GlobalControlsViewModel(BuildPulse, MetronomeMode.Basic);
            TimeSignatureControls = new TimeSignatureControlsViewModel();        
        }

        public override void OnClosing()
        {
            GlobalControls.OnClosing();
        }

        private async Task<ISampleProvider> BuildPulse()
        {
            var settings = await UserSettings.Get();

            var pulse = ClickTrackBuilder.BuildSinglePulse(
                new BarInfo(TimeSignatureControls.TempoNumericBoxValue,
                            TimeSignatureControls.SelectedBeats,
                            TimeSignatureControls.SelectedNoteLength),
                settings.ClickSettings
            );

            return pulse;
        }
    }
}
