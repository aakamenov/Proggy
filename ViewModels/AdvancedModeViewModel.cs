using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Proggy.Core;
using Proggy.Infrastructure;
using Proggy.ViewModels.CollectionItems;
using Proggy.Views;
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
            globalControls = new GlobalControlsViewModel(new SinglePulseTrackBuilder(), MetronomeMode.Advanced);
            globalControls.BarInfo.Add(new BarInfo(120, 4, 4));

            Items = new ObservableCollection<ClickTrackGridItem>();
            Items.Add(new BarInfoGridItem(globalControls.BarInfo.First()));           
            Items.Add(new AddButtonGridItem());
        }

        public async void OnItemClicked(BarInfoGridItem item)
        {
            //Add button pressed
            if(item is null)
            {
                var lastItem = globalControls.BarInfo.Last();

                var info = lastItem.DeepCopy();
                globalControls.BarInfo.Add(info);
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
                    var index = globalControls.BarInfo.IndexOf(item.BarInfo);
                    globalControls.BarInfo[index] = result.BarInfo;

                    item.BarInfo = result.BarInfo;
                }
            }
        }
    }
}
