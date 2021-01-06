using System;
using Proggy.Core;
using ReactiveUI;

namespace Proggy.ViewModels.Dialogs
{
    public class TimeSignatureDialogViewModel : BaseDialogViewModel
    {
        public TimeSignatureControlsViewModel TimeSignatureControls { get; }
        public BarInfo BarInfo { get; private set; }

        public TimeSignatureDialogViewModel(BarInfo info)
        {
            BarInfo = info;

            TimeSignatureControls = new TimeSignatureControlsViewModel()
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

        protected override void Ok()
        {
            Close.Invoke(new DialogResult<BarInfo>(DialogAction.OK, BarInfo));
        }

        protected override void Cancel()
        {
            Close.Invoke(new DialogResult<BarInfo>(DialogAction.Cancel, BarInfo.Default));
        }
    }
}
