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
using Avalonia;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using NAudio.Wave;

namespace Proggy.ViewModels
{
    public class AdvancedModeViewModel : ViewModelBase
    {
        public const int MaxRowsOrCols = 5;

        public ObservableCollection<ClickTrackGridItem> Items { get; }

        public ListItem<float>[] PlaybackSpeeds { get; }

        public ListItem<float> SelectedPlaybackSpeed
        {
            get => selectedPlaybackSpeed;
            set => this.RaiseAndSetIfChanged(ref selectedPlaybackSpeed, value);
        }

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
        private int firstItemIndex;
        private int lastItemIndex;
        private string trackName;
        private AccurateTimer timer;
        private ListItem<float> selectedPlaybackSpeed;

        public AdvancedModeViewModel()
        {
            PlaybackSpeeds = new ListItem<float>[(110 - ClickTrackBuilder.MinPlaybackSpeedPercent) / 10];

            for (var i = 0; i < PlaybackSpeeds.Length; i++)
            {
                var value = ClickTrackBuilder.MinPlaybackSpeedPercent + (i * 10);
                PlaybackSpeeds[PlaybackSpeeds.Length - 1 - i] = new ListItem<float>($"{value}%", value);
            }

            selectedPlaybackSpeed = PlaybackSpeeds.First();

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

        public async void Export()
        {
            var dialog = new SaveFileDialog()
            {
                Title = "Export"
            };

            var filter = new FileDialogFilter()
            {
                Name = "WAVE"
            };
            filter.Extensions.Add("wav");

            dialog.Filters.Add(filter);

            var lifeTime = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

            var path = await dialog.ShowAsync(lifeTime.MainWindow);

            if (string.IsNullOrEmpty(path))
                return;

            var oldValue = loop;

            loop = false;
            var track = await BuildClickTrackAsync();
            loop = oldValue;

            try
            {
                WaveFileWriter.CreateWaveFile16(path, track);
            }
            catch(Exception e)
            {
                await WindowNavigation.ShowErrorMessageAsync(e);
            }
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
            ApplySelection(false, true);
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

            if (result == DialogAction.OK)
                await Save();
        }

        private async Task<ISampleProvider> BuildClickTrackAsync()
        {
            void PopulateArray(int start, int end, BarInfo[] array)
            {
                var index = 0;

                for (var i = start; i < end; i++)
                {
                    var bar = (BarInfoGridItem)Items[i];
                    array[index] = bar.BarInfo;

                    index++;
                }
            }

            BarInfo[] infos = null;

            if(Selection.HasSelection)
            {
                var range = Selection.Range;
                infos = new BarInfo[range.End - range.Start + 1];

                PopulateArray(range.Start, range.End + 1, infos);
            }
            else if(Selection.IsSelecting) //Play from the selected bar to the end
            {
                var range = Selection.Range;
                infos = new BarInfo[Items.Count - range.Start - 1];

                PopulateArray(range.Start, Items.Count - 1, infos);
            }
            else
                infos = Items.OfType<BarInfoGridItem>().Select(x => x.BarInfo).ToArray();

            var settings = await UserSettings.Get();

            return await Task.Run(() => ClickTrackBuilder.BuildClickTrack(
                infos, 
                settings.ClickSettings,
                SelectedPlaybackSpeed.Value,
                precount,
                loop)
            );
        }

        private void ApplySelection(bool select, bool removeSelection)
        {
            if (Selection.HasSelection)
            {
                var range = Selection.Range;

                for (var i = range.Start; i <= range.End; i++)
                {
                    var bar = (BarInfoGridItem)Items[i];
                    bar.IsSelected = select;
                }
            }
            else if (Selection.IsSelecting)
            {
                var bar = (BarInfoGridItem)Items[Selection.Range.Start];
                bar.IsSelected = select;
            }

            if (removeSelection)
                Selection.RemoveSelection();
        }

        private void OnMetronomePlaybackStateChanged(MetronomePlaybackStateChanged msg)
        {
            if (msg.State == MetronomePlaybackState.Playing)
            {
                ApplySelection(false, false);

                Items.RemoveAt(Items.Count - 1); //Remove add button

                CanUseContextMenu = false;

                if (Items.Count > 1)
                {
                    if(Selection.HasSelection)
                    {
                        var range = Selection.Range;

                        firstItemIndex = range.Start;
                        lastItemIndex = range.End;
                    }
                    else if(Selection.IsSelecting)
                    {
                        firstItemIndex = Selection.Range.Start;
                        lastItemIndex = Items.Count - 1;
                    }
                    else
                    {
                        firstItemIndex = 0;
                        lastItemIndex = Items.Count - 1;
                    }

                    currentItemIndex = firstItemIndex;

                    if (precount)
                    {
                        var first = (BarInfoGridItem)Items.First();

                        timer.Interval = new BarInfo(first.BarInfo.Tempo, 4, 4)
                            .GetInterval(SelectedPlaybackSpeed.Value) * 4;
                    }
                    else
                        timer.Interval = 0;

                    timer.Start();
                }
            }
            else
            {
                if (Items.Count > 1)
                {
                    for (var i = 0; i < Items.Count; i++)
                    {
                        var bar = (BarInfoGridItem)Items[i];
                        bar.IsSelected = false;
                    }

                    timer.Stop();
                }

                ApplySelection(true, false);

                Items.Add(new AddButtonGridItem());

                CanUseContextMenu = true;
            }
        }

        private async void UpdateCurrentBar()
        {
            var current = (BarInfoGridItem)Items[currentItemIndex];
            current.IsSelected = true;

            if (currentItemIndex > firstItemIndex)
            {
                var previous = (BarInfoGridItem)Items[currentItemIndex - 1];
                previous.IsSelected = false;
            }
            else if (currentItemIndex == firstItemIndex)
            {
                var lastItem = (BarInfoGridItem)Items[lastItemIndex];
                lastItem.IsSelected = false;
            }

            if (currentItemIndex == lastItemIndex)
                currentItemIndex = firstItemIndex;
            else
                currentItemIndex++;

            timer.Interval = current.BarInfo.GetInterval(SelectedPlaybackSpeed.Value) * current.BarInfo.Beats;

            //This should be done last
            if(currentItemIndex % MaxRowsOrCols == 1)
                await Dispatcher.UIThread.InvokeAsync(() => ScrollToBar(currentItemIndex));
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
