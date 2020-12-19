using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Proggy.Core;
using Proggy.Models;
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
        public ObservableCollection<ClickTrackGridItem> Items { get; }

        public Action<int> ScrollToBar { get; set; }

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

        public string TrackName
        {
            get => trackName;
            set => this.RaiseAndSetIfChanged(ref trackName, value);
        }

        public GlobalControlsViewModel GlobalControls => globalControls;

        private readonly GlobalControlsViewModel globalControls;
        private bool loop;
        private bool precount;
        private int currentItemIndex;
        private AccurateTimer timer;
        private string trackName;

        public AdvancedModeViewModel()
        {
            globalControls = new GlobalControlsViewModel(BuildClickTrackAsync, MetronomeMode.Advanced);

            Items = new ObservableCollection<ClickTrackGridItem>();
            InitializeTrack();

            Loop = true;

            MessageBus.Current.Listen<MetronomePlaybackStateChanged>().Subscribe(OnMetronomePlaybackStateChanged);

            timer = new AccurateTimer(UpdateCurrentBar);

            SetNewTrackName();
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
                var result = await WindowNavigation.ShowDialogAsync(() => 
                {
                    return new TimeSignatureDialogViewModel(item.BarInfo);
                });

                if (result.IsConfirm)
                    item.BarInfo = result.BarInfo;
            }
        }

        public void DeleteItem(BarInfoGridItem item)
        {
            if (Items.Count > 2)
                Items.Remove(item);
        }

        public async void Save()
        {
            var infos = Items.OfType<BarInfoGridItem>().Select(x => x.BarInfo).ToArray();

            try
            {
                await ClickTrackFile.Save(infos, TrackName);
            }
            catch(Exception e)
            {
                await WindowNavigation.ShowErrorMessageAsync(e);
            }
        }

        public async void Open()
        {
            var result = await WindowNavigation.ShowDialogAsync(() => 
            {
                return new OpenClickTrackDialog(ClickTrackFile.Enumerate());
            });

            if(result.IsConfirm)
            {
                try
                {
                    var track = await ClickTrackFile.Load(result.SelectedTrack);

                    Items.Clear();

                    foreach (var info in track)
                        Items.Add(new BarInfoGridItem(info));

                    Items.Add(new AddButtonGridItem());

                    TrackName = result.SelectedTrack;
                }
                catch(Exception e)
                {
                    await WindowNavigation.ShowErrorMessageAsync(e);
                }
            }
        }

        public void New()
        {
            Items.Clear();
            InitializeTrack();
            SetNewTrackName();
        }

        private async Task<ISampleProvider> BuildClickTrackAsync()
        {
            var infos = Items.OfType<BarInfoGridItem>().Select(x => x.BarInfo).ToArray();
            var settings = await UserSettings.Get();

            return await ClickTrackBuilder.BuildClickTrackAsync(infos, settings.ClickSettings, precount, loop);
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

        private async void UpdateCurrentBar()
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

            await Dispatcher.UIThread.InvokeAsync(() => ScrollToBar(currentItemIndex));
        }

        private void DeselectAll()
        {
            foreach (var item in Items.OfType<BarInfoGridItem>())
                item.IsSelected = false;
        }

        private void SetNewTrackName()
        {
            const string newTrack = "New Track";

            var names = ClickTrackFile.Enumerate()
                .Where(x => x.StartsWith(newTrack))
                .ToArray();

            if (names.Length > 0)
            {
                var counter = names.Length;
                var name = string.Empty;

                do
                {
                    counter++;
                    name = $"{newTrack} {counter}";
                }
                while (File.Exists(Path.Combine(ClickTrackFile.FolderPath, $"{name}{ClickTrackFile.FileExtension}")));

                TrackName = name;
            }
            else
                TrackName = newTrack;
        }

        private void InitializeTrack()
        {
            Items.Add(new BarInfoGridItem(new BarInfo(120, 4, 4)));
            Items.Add(new AddButtonGridItem());
        }
    }
}
