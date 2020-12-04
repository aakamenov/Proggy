using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Proggy.Core;
using Proggy.Infrastructure;
using Proggy.Infrastructure.Events;
using Proggy.ViewModels.CollectionItems;
using ReactiveUI;
using NAudio.Wave;

namespace Proggy.ViewModels
{
    public class AdvancedModeViewModel : ViewModelBase
    {
        private const int TimerIntervalMs = 10;

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
        private int currentItemIndex;

        private AccurateTimer timer;

        public AdvancedModeViewModel()
        {
            globalControls = new GlobalControlsViewModel(BuildClickTrackAsync, MetronomeMode.Advanced);

            Items = new ObservableCollection<ClickTrackGridItem>
            {
                new BarInfoGridItem(new BarInfo(120, 4, 4)),
                new AddButtonGridItem()
            };

            Loop = true;

            MessageBus.Current.Listen<MetronomePlaybackStateChanged>().Subscribe(OnMetronomePlaybackStateChanged);

            timer = new AccurateTimer(UpdateCurrentBar);
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
            var infos = Items.OfType<BarInfoGridItem>().Select(x => x.BarInfo).ToArray();
            return await ClickTrackBuilder.BuildClickTrackAsync(infos, precount, loop);
        }

        private void OnMetronomePlaybackStateChanged(MetronomePlaybackStateChanged msg)
        {
            if (msg.State == MetronomePlaybackState.Playing)
            {
                if (Items.Count == 2)
                    return;

                currentItemIndex = 0;

                if(precount)
                {
                    var first = (BarInfoGridItem)Items.First();
                    timer.Interval = new BarInfo(first.BarInfo.Tempo, 4, 4).Interval * 4;
                }
                else
                    timer.Interval = 0;

                timer.Start();
            }
            else
            {
                timer.Stop();
                DeselectAll();
            }
        }

        private void UpdateCurrentBar()
        {
            var current = (BarInfoGridItem)Items[currentItemIndex];
            current.IsSelected = true;

            if (currentItemIndex > 0)
            {
                var previous = (BarInfoGridItem)Items[currentItemIndex - 1];
                previous.IsSelected = false;
            }
            else if (currentItemIndex == 0)
            {
                var lastItem = (BarInfoGridItem)Items[Items.Count - 2];
                lastItem.IsSelected = false;
            }

            if (currentItemIndex == Items.Count - 2)
                currentItemIndex = 0;
            else
                currentItemIndex++;

            timer.Interval = current.BarInfo.Interval * current.BarInfo.Beats;
        }

        private void DeselectAll()
        {
            foreach (var item in Items.OfType<BarInfoGridItem>())
                item.IsSelected = false;
        }
    }
}
