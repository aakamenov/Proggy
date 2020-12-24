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
        public const int MaxRowsOrCols = 5;

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

        public bool CanUseContextMenu
        {
            get => canUseContextMenu && Items.Count > 2;
            set => this.RaiseAndSetIfChanged(ref canUseContextMenu, value);
        }

        public Selection Selection { get; }

        public GlobalControlsViewModel GlobalControls { get; }

        private IDisposable playbackChangedSub;

        private bool loop;
        private bool precount;
        private bool canUseContextMenu;
        private bool pendingChanges;
        private int currentItemIndex;
        private string trackName;
        private AccurateTimer timer;

        public AdvancedModeViewModel()
        {
            GlobalControls = new GlobalControlsViewModel(BuildClickTrackAsync, MetronomeMode.Advanced);
            Selection = new Selection();

            Items = new ObservableCollection<ClickTrackGridItem>();
            InitializeTrack();

            Loop = true;

            canUseContextMenu = true;

            playbackChangedSub = MessageBus.Current.Listen<MetronomePlaybackStateChanged>()
                                                   .Subscribe(OnMetronomePlaybackStateChanged);

            timer = new AccurateTimer(UpdateCurrentBar);

            SetNewTrackName();
        }

        public async void OnItemClicked(BarInfoGridItem item)
        {
            if (AudioPlayer.Instance.IsPlaying)
                return;

            pendingChanges = true;

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

        public void Delete(BarInfoGridItem item)
        {
            var index = Items.IndexOf(item);

            if (Selection.HasSelection && Selection.Contains(index))
            {
                var range = Selection.Range;

                for (var i = range.Start; i <= range.End; i++)
                    Items.RemoveAt(range.Start);

                if (Items.Count < 2)
                    Items.Insert(0, new BarInfoGridItem(BarInfo.Default));

                Selection.RemoveSelection();           
            }
            else if(Selection.HasSelection) //Item is outside of selected range
            {
                if (index < Selection.Range.Start)
                {
                    Selection.Range.Start--;
                    Selection.Range.End--;
                }

                Items.RemoveAt(index);
            }
            else if(Selection.IsSelecting)
            {
                if(index < Selection.Range.Start)
                    Selection.Range.Start--;

                Items.RemoveAt(index);
            }
            else //No selection
                Items.RemoveAt(index);

            pendingChanges = true;
        }

        public async Task Save()
        {
            var infos = Items.OfType<BarInfoGridItem>().Select(x => x.BarInfo).ToArray();
            
            try
            {
                await ClickTrackFile.Save(infos, TrackName);

                pendingChanges = false;
            }
            catch(Exception e)
            {
                await WindowNavigation.ShowErrorMessageAsync(e);
            }
        }

        public async void Open()
        {
            GlobalControls.Stop();

            var result = await WindowNavigation.ShowDialogAsync(() => 
            {
                return new OpenClickTrackDialog(ClickTrackFile.Enumerate());
            });

            if(result.IsConfirm)
            {
                try
                {
                    var track = await ClickTrackFile.Load(result.SelectedTrack);

                    await PromptSave();

                    Items.Clear();

                    foreach (var info in track)
                        Items.Add(new BarInfoGridItem(info));

                    Items.Add(new AddButtonGridItem());

                    TrackName = result.SelectedTrack;

                    Selection.RemoveSelection();
                }
                catch(Exception e)
                {
                    await WindowNavigation.ShowErrorMessageAsync(e);
                }
            }
        }

        public async void New()
        {
            if (AudioPlayer.Instance.IsPlaying)
                return;

            await PromptSave();

            pendingChanges = false;
            Selection.RemoveSelection();

            Items.Clear();
            InitializeTrack();
            SetNewTrackName();
        }

        public void Select(BarInfoGridItem item)
        {
            if (Selection.IsSelecting)
                EndSelection(item);
            else
                BeginSelection(item);
        }

        public void BeginSelection(BarInfoGridItem item)
        {
            ClearSelection();

            item.IsSelected = true;

            Selection.Begin(Items.IndexOf(item));
        }

        public void EndSelection(BarInfoGridItem item)
        {
            Selection.Finalize(Items.IndexOf(item));

            var range = Selection.Range;

            if(range.Start == range.End)
            {
                item.IsSelected = false;
                Selection.RemoveSelection();

                return;
            }

            for(var i = range.Start; i <= range.End; i++)
            {
                var bar = (BarInfoGridItem)Items[i];
                bar.IsSelected = true;
            }
        }

        public void ClearSelection()
        {
            if(Selection.HasSelection)
            {
                var range = Selection.Range;

                for (var i = range.Start; i <= range.End; i++)
                {
                    var bar = (BarInfoGridItem)Items[i];
                    bar.IsSelected = false;
                }
            }
            else if(Selection.IsSelecting)
            {
                var bar = (BarInfoGridItem)Items[Selection.Range.Start];
                bar.IsSelected = false;
            }

            Selection.RemoveSelection();
        }

        public override void OnClosing()
        {
            playbackChangedSub.Dispose();
            GlobalControls.OnClosing();
        }

        public async Task PromptSave()
        {
            if (!pendingChanges)
                return;

            var result = await WindowNavigation.PromptAsync("Do you wish to save your track?", "Save?");

            if (result.Result == DialogAction.OK)
                await Save();
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
                Items.RemoveAt(Items.Count - 1); //Remove add button

                CanUseContextMenu = false;

                DeselectAll();

                if (Items.Count > 1)
                {
                    currentItemIndex = 0;

                    if (precount)
                    {
                        var first = (BarInfoGridItem)Items.First();
                        timer.Interval = new BarInfo(first.BarInfo.Tempo, 4, 4).Interval * 4;
                    }
                    else
                        timer.Interval = 0;

                    timer.Start();
                }
            }
            else
            {
                Items.Add(new AddButtonGridItem());

                CanUseContextMenu = true;

                if (Items.Count > 1)
                {
                    timer.Stop();
                    DeselectAll();
                }
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
                var lastItem = (BarInfoGridItem)Items[Items.Count - 1];
                lastItem.IsSelected = false;
            }

            if (currentItemIndex == Items.Count - 1)
                currentItemIndex = 0;
            else
                currentItemIndex++;

            timer.Interval = current.BarInfo.Interval * current.BarInfo.Beats;

            //This should be done last
            if(currentItemIndex % MaxRowsOrCols == 1)
                await Dispatcher.UIThread.InvokeAsync(() => ScrollToBar(currentItemIndex));
        }

        private void DeselectAll()
        {
            foreach (var item in Items.OfType<BarInfoGridItem>())
                item.IsSelected = false;

            Selection.RemoveSelection();
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
            Items.Add(new BarInfoGridItem(BarInfo.Default));
            Items.Add(new AddButtonGridItem());
        }
    }
}
