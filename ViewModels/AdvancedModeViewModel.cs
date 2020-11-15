using System;
using System.Collections.ObjectModel;
using System.Linq;
using Proggy.Core;
using Proggy.Infrastructure;
using Proggy.ViewModels.CollectionItems;
using ReactiveUI;

namespace Proggy.ViewModels
{
    public class AdvancedModeViewModel : ViewModelBase
    {
        public ObservableCollection<ClickTrackGridItem> Items { get; }

        public GlobalControlsViewModel GlobalControls => globalControls;
        private readonly GlobalControlsViewModel globalControls;

        public AdvancedModeViewModel()
        {
            globalControls = new GlobalControlsViewModel(new MultiPulseTrackBuilder(), MetronomeMode.Advanced)
            {
                Loop = true
            };
            globalControls.ClickTrack.Add(new BarInfo(120, 4, 4));
            
            Items = new ObservableCollection<ClickTrackGridItem>();
            Items.Add(new BarInfoGridItem(globalControls.ClickTrack.First()));           
            Items.Add(new AddButtonGridItem());
        }

        public async void OnItemClicked(BarInfoGridItem item)
        {
            //Add button pressed
            if(item is null)
            {
                var lastItem = globalControls.ClickTrack.Last();

                var info = lastItem.DeepCopy();
                globalControls.ClickTrack.Add(info);
                Items.Insert(Items.Count - 1, new BarInfoGridItem(info));
            }
            else
            {
                var result = await WindowNavigation.NavigateAsync(() => 
                {
                    return new TimeSignatureDialogViewModel(item.BarInfo.DeepCopy());
                });

                if (result.WasClosedFromView)
                {
                    var index = globalControls.ClickTrack.IndexOf(item.BarInfo);
                    globalControls.ClickTrack[index] = result.BarInfo;

                    item.BarInfo = result.BarInfo;
                }
            }
        }

        public void DeleteItem(BarInfoGridItem item)
        {
            if (Items.Count > 2)
            {
                Items.Remove(item);
                globalControls.ClickTrack.Remove(item.BarInfo);
            }
        }
    }
}
