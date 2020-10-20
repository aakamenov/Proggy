using System;
using System.Reactive.Linq;
using Proggy.Core;
using Proggy.Infrastructure;
using ReactiveUI;

namespace Proggy.ViewModels
{
    public class BasicModeViewModel : ViewModelBase
    {
        const short MAX_BEATS = 17;
        readonly short[] noteLengths = { 2, 4, 8, 16, 32 };

        public short[] Beats => beats;
        public short[] NoteLengths => noteLengths;

        public short SelectedBeats 
        {
            get => selectedBeats;
            set => this.RaiseAndSetIfChanged(ref selectedBeats, value);
        }

        public short SelectedNoteLength
        {
            get => selectedNoteLength;
            set => this.RaiseAndSetIfChanged(ref selectedNoteLength, value);
        }

        public short TempoNumericBoxValue
        {
            get => tempoNumericBoxValue;
            set => this.RaiseAndSetIfChanged(ref tempoNumericBoxValue, value);
        }

        public GlobalControlsViewModel GlobalControls => globalControls;
        private readonly GlobalControlsViewModel globalControls;

        private short tempoNumericBoxValue;
        private short selectedNoteLength;
        private short selectedBeats;
        private short[] beats;

        public BasicModeViewModel()
        {
            beats = new short[MAX_BEATS - 1];

            for (short i = 2; i <= MAX_BEATS; i++)
                beats[i - 2] = i;
            
            selectedBeats = 4;
            selectedNoteLength = 4;
            tempoNumericBoxValue = 120;
            
            globalControls = new GlobalControlsViewModel(new SinglePulseTrackBuilder(), MetronomeMode.Basic);
            
            this.WhenAnyValue(x => x.SelectedBeats, x => x.SelectedNoteLength, x => x.TempoNumericBoxValue)
                .Subscribe(x => SetTimeSignature(x.Item1, x.Item2, x.Item3));
        }

        private void SetTimeSignature(short beats, short noteLength, short tempo)
        {
            globalControls.BarInfo.Clear();
            globalControls.BarInfo.Add(new BarInfo(tempo, beats, noteLength));
        }
    }
}
