using System;
using System.Reactive.Linq;
using Proggy.Core;
using Proggy.Infrastructure;
using ReactiveUI;

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
            globalControls = new GlobalControlsViewModel(new SinglePulseTrackBuilder(), MetronomeMode.Basic);
            timeSignatureControls = new TimeSignatureControlsViewModel();
            
            this.WhenAnyValue(x => x.TimeSignatureControls.SelectedBeats,
                              x => x.TimeSignatureControls.SelectedNoteLength,
                              x => x.TimeSignatureControls.TempoNumericBoxValue)
                .Subscribe(x => SetTimeSignature(x.Item1, x.Item2, x.Item3));
        }

        private void SetTimeSignature(short beats, short noteLength, short tempo)
        {
            globalControls.ClickTrack.Clear();
            globalControls.ClickTrack.Add(new BarInfo(tempo, beats, noteLength));
        }
    }
}
