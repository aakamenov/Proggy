using ReactiveUI;
using Proggy.Core;

namespace Proggy.ViewModels
{
    public class TimeSignatureControlsViewModel : ViewModelBase
    {
        public byte[] Beats { get; }
        public byte[] NoteLengths => BarInfo.NoteLengths;

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

        public TimeSignatureControlsViewModel()
        {
            Beats = new byte[BarInfo.MaxBeats - 1];

            for (byte i = 2; i <= BarInfo.MaxBeats; i++)
                Beats[i - 2] = i;

            var info = BarInfo.Default;

            selectedBeats = info.Beats;
            selectedNoteLength = info.NoteLength;
            tempoNumericBoxValue = info.Tempo;
        }
    }
}
