using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace Proggy.ViewModels
{
    public class TimeSignatureControlsViewModel : ViewModelBase
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

        private short tempoNumericBoxValue;
        private short selectedNoteLength;
        private short selectedBeats;
        private short[] beats;

        public TimeSignatureControlsViewModel()
        {
            beats = new short[MAX_BEATS - 1];

            for (short i = 2; i <= MAX_BEATS; i++)
                beats[i - 2] = i;

            selectedBeats = 4;
            selectedNoteLength = 4;
            tempoNumericBoxValue = 120;
        }
    }
}
