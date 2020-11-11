using System;
using Proggy.Core;
using Proggy.Infrastructure;
using ReactiveUI;

namespace Proggy.ViewModels
{
    public class TimeSignatureDialogViewModel : ViewModelBase, IDialog
    {
        public Action Close { get; set; }
        public bool WasClosedFromView { get; set; }
        public TimeSignatureControlsViewModel TimeSignatureControls => timeSignatureControls;
        public BarInfo BarInfo { get; private set; }

        private readonly TimeSignatureControlsViewModel timeSignatureControls;

        public TimeSignatureDialogViewModel(BarInfo info)
        {
            BarInfo = info;

            timeSignatureControls = new TimeSignatureControlsViewModel()
            {
                TempoNumericBoxValue = info.Tempo,
                SelectedBeats = info.Beats,
                SelectedNoteLength = info.NoteLength
            };

            this.ObservableForProperty(x => x.TimeSignatureControls.TempoNumericBoxValue)
                .Subscribe(x => BarInfo = new BarInfo(
                    x.Value, 
                    TimeSignatureControls.SelectedBeats, 
                    TimeSignatureControls.SelectedNoteLength
                 ));

            this.ObservableForProperty(x => x.TimeSignatureControls.SelectedBeats)
                .Subscribe(x => BarInfo = new BarInfo(
                    TimeSignatureControls.TempoNumericBoxValue,
                    x.Value,
                    TimeSignatureControls.SelectedNoteLength
                 ));

             this.ObservableForProperty(x => x.TimeSignatureControls.SelectedNoteLength)
                .Subscribe(x => BarInfo = new BarInfo(
                    TimeSignatureControls.TempoNumericBoxValue,
                    TimeSignatureControls.SelectedBeats,
                    x.Value
                 ));
        }

        public void Save()
        {
            Close?.Invoke();
        }
    }
}
