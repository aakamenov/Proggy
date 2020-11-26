using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reactive.Linq;
using Proggy.Core;
using Proggy.Infrastructure;
using Proggy.Infrastructure.Events;
using Proggy.ViewModels.CollectionItems;
using ReactiveUI;
using Avalonia.Threading;
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

        private DispatcherTimer timer;
        private Stopwatch stopwatch;
        private long lastUpdate;
        private int currentInterval;
        private int currentItemIndex;

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

            timer = new DispatcherTimer();
            timer.Tick += OnTimerElapsed;

            stopwatch = new Stopwatch();
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

                currentInterval = precount ? 2000 /*4/4 120BPM*/ : 0;
                currentItemIndex = 0;

                stopwatch.Start();

                timer.Interval = TimeSpan.FromMilliseconds(TimerIntervalMs);
                timer.Start();
            }
            else
            {
                stopwatch.Stop();
                timer.Stop();
                DeselectAll();
            }
        }

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            if (!(stopwatch.ElapsedMilliseconds - lastUpdate >= currentInterval - TimerIntervalMs))
                return;
            Debug.WriteLine($"Update: {stopwatch.ElapsedMilliseconds - lastUpdate} ms");
            lastUpdate = stopwatch.ElapsedMilliseconds;
            
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

            currentInterval = current.BarInfo.Interval * current.BarInfo.Beats;
        }

        private void DeselectAll()
        {
            foreach (var item in Items.OfType<BarInfoGridItem>())
                item.IsSelected = false;
        }
    }
}
