using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace Proggy.ViewModels
{
    public class TimeSignatureControlsViewModel : ViewModelBase
    {
        const byte MAX_BEATS = 32;
        readonly byte[] noteLengths = { 2, 4, 8, 16, 32 };

        public byte[] Beats => beats;
        public byte[] NoteLengths => noteLengths;

        public byte SelectedBeats
        {
            get => selectedBeats;
            set => this.RaiseAndSetIfChanged(ref selectedBeats, value);
        }

        public byte SelectedNoteLength
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
        private byte selectedNoteLength;
        private byte selectedBeats;
        private byte[] beats;

        public TimeSignatureControlsViewModel()
        {
            beats = new byte[MAX_BEATS - 1];

            for (byte i = 2; i <= MAX_BEATS; i++)
                beats[i - 2] = i;

            selectedBeats = 4;
            selectedNoteLength = 4;
            tempoNumericBoxValue = 120;
        }
    }
}
