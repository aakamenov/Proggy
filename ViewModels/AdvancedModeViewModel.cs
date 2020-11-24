using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Proggy.Core;
using Proggy.Infrastructure;
using Proggy.ViewModels.CollectionItems;
using ReactiveUI;
using NAudio.Wave;

namespace Proggy.ViewModels
{
    public class AdvancedModeViewModel : ViewModelBase
    {
        public ObservableCollection<ClickTrackGridItem> Items { get; }

        public bool Loop
        {
            get => loop;
            set => this.RaiseAndSetIfChanged(ref loop, value);
        }
        public bool Precount
        {
            get => precount;
            set => this.RaiseAndSetIfChanged(ref precount, value);
        }

        public GlobalControlsViewModel GlobalControls => globalControls;

        private readonly GlobalControlsViewModel globalControls;
        private bool loop;
        private bool precount;

        public AdvancedModeViewModel()
        {
            globalControls = new GlobalControlsViewModel(BuildClickTrackAsync, MetronomeMode.Advanced);

            Items = new ObservableCollection<ClickTrackGridItem>
            {
                new BarInfoGridItem(new BarInfo(120, 4, 4)),
                new AddButtonGridItem()
            };

            Loop = true;
        }

        public async void OnItemClicked(BarInfoGridItem item)
        {
            //Add button pressed
            if(item is null)
            {
                var lastItem = (BarInfoGridItem)Items[Items.Count - 2];
                Items.Insert(Items.Count - 1, new BarInfoGridItem(lastItem.BarInfo));
            }
            else
            {
                var result = await WindowNavigation.NavigateAsync(() => 
                {
                    return new TimeSignatureDialogViewModel(item.BarInfo.DeepCopy());
                });

                if (result.WasClosedFromView)
                    item.BarInfo = result.BarInfo;
            }
        }

        public void DeleteItem(BarInfoGridItem item)
        {
            if (Items.Count > 2)
                Items.Remove(item);
        }

        private async Task<ISampleProvider> BuildClickTrackAsync()
        {
            var infos = Items.Take(Items.Count - 1).Cast<BarInfoGridItem>().Select(x => x.BarInfo).ToArray();
            return await ClickTrackBuilder.BuildClickTrackAsync(infos, precount, loop);
        }
    }
}
