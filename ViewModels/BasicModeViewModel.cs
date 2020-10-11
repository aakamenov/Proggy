using System;
using System.Collections.Generic;
using System.Text;
using Proggy.Controls;
using Proggy.Core;
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
            set 
            { 
                this.RaiseAndSetIfChanged(ref selectedBeats, value);
                SetTimeSignature();
            }
        }

        public short SelectedNoteLength
        {
            get => selectedNoteLength;
            set 
            { 
                this.RaiseAndSetIfChanged(ref selectedNoteLength, value);
                SetTimeSignature();
            }
        }

        public string TempoTextBoxText
        {
            get => tempoTextBoxText;
            set 
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;

                this.RaiseAndSetIfChanged(ref tempoTextBoxText, value);
                SetTimeSignature();
            }
        }

        public GlobalControlsViewModel GlobalControls => globalControls;
        private readonly GlobalControlsViewModel globalControls;

        private string tempoTextBoxText;
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
            tempoTextBoxText = "120";

            globalControls = new GlobalControlsViewModel(new SinglePulseTrackBuilder());
            SetTimeSignature();
        }

        private void SetTimeSignature()
        {
            globalControls.BarInfo.Clear();
            globalControls.BarInfo.Add(new BarInfo(int.Parse(tempoTextBoxText), selectedBeats, selectedNoteLength));
        }
    }
}
