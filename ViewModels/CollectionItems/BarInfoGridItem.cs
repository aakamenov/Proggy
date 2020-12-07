using System;
using System.Reactive.Linq;
using ReactiveUI;
using Proggy.Core;

namespace Proggy.ViewModels.CollectionItems
{
    public class BarInfoGridItem : ClickTrackGridItem
    {
        public bool IsSelected
        {
            get => isSelected;
            set => this.RaiseAndSetIfChanged(ref isSelected, value);
        }

        public BarInfo BarInfo
        {
            get => barInfo;
            set => this.RaiseAndSetIfChanged(ref barInfo, value);
        }

        public string TimeSignature => $"{BarInfo.Beats}/{BarInfo.NoteLength}";

        private BarInfo barInfo;
        private bool isSelected;

        public BarInfoGridItem(BarInfo info)
        {
            BarInfo = info;
            this.WhenAnyValue(x => x.BarInfo).Subscribe(_ => this.RaisePropertyChanged(nameof(TimeSignature)));
        }
    }
}
