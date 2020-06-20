using Proggy.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proggy.ViewModels
{
    public class BasicModeViewModel : ViewModelBase
    {
        const short MAX_BEATS = 17;
        readonly short[] noteLengths = { 2, 4, 8, 16, 32, 64 };

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

        public GlobalControlsViewModel GlobalControls => globalControls;
        private readonly GlobalControlsViewModel globalControls;

        private short selectedNoteLength;
        private short selectedBeats;
        private short[] beats;

        public BasicModeViewModel()
        {
            beats = new short[MAX_BEATS];

            for (short i = 2; i <= MAX_BEATS; i++)
                beats[i - 2] = i;

            SelectedBeats = 4;
            SelectedNoteLength = 4;

            globalControls = new GlobalControlsViewModel();
        }
    }
}
