using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Windows;
using Proggy.Core;
using Proggy.Models;
using Proggy.Infrastructure;
using Proggy.Infrastructure.Events;
using Proggy.Infrastructure.Commands;
using Proggy.ViewModels.CollectionItems;
using Proggy.ViewModels.Dialogs;
using ReactiveUI;
using NAudio.Wave;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace Proggy.ViewModels
{
    public class AdvancedModeViewModel : ViewModelBase
    {
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

        public bool IsDialogOpen { get; set; }

        public Selection Selection { get; }

        public GlobalControlsViewModel GlobalControls { get; }

        public SnackbarMessageQueue MessageQueue { get; }

        public Command<BarInfoGridItem> OnItemClickedCommand { get; }
        public Command<BarInfoGridItem> DeleteCommand { get; }
        public Command<BarInfoGridItem> SelectCommand { get; }
        public Command<BarInfoGridItem> BeginSelectionCommand { get; }
        public Command<BarInfoGridItem> EndSelectionCommand { get; }
        public Command<bool> SaveCommand { get; }
        public Command NewCommand { get; }
        public Command OpenCommand { get; }
        public Command ExportCommand { get; }
        public Command ClearSelectionCommand { get; }

        private readonly IDisposable playbackChangedSub;

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
            GlobalControls.ToggleCommand.CanExecuteFunc = (p) => !IsDialogOpen;

            Selection = new Selection();

            Items = new ObservableCollection<ClickTrackGridItem>();
            InitializeTrack();

            Loop = true;

            canUseContextMenu = true;

            playbackChangedSub = MessageBus.Current.Listen<MetronomePlaybackStateChanged>()
                                                   .Subscribe(OnMetronomePlaybackStateChanged);

            MessageQueue = new SnackbarMessageQueue();

            timer = new AccurateTimer(UpdateCurrentBar);

            SetNewTrackName();

            OnItemClickedCommand = new Command<BarInfoGridItem>(OnItemClicked);
            DeleteCommand = new Command<BarInfoGridItem>(Delete);
            SelectCommand = new Command<BarInfoGridItem>(Select);
            BeginSelectionCommand = new Command<BarInfoGridItem>(BeginSelection);
            EndSelectionCommand = new Command<BarInfoGridItem>(EndSelection);
            ClearSelectionCommand = new Command(ClearSelection);
            SaveCommand = new Command<bool>(Save);
            NewCommand = new Command(New);
            OpenCommand = new Command(Open);
            ExportCommand = new Command(Export);
        }

        public override void OnClosing()
        {
            playbackChangedSub.Dispose();
            GlobalControls.OnClosing();
            MessageQueue.Dispose();
        }

        public async Task PromptSave()
        {
            if (!pendingChanges)
                return;

            if(IsDialogOpen)
                DialogHost.Close(null);

            var result = await WindowNavigation.ShowDialog(
                new AlertDialogViewModel("Do you wish to save your track?", true));

            if (result.Action == DialogAction.OK)
                SaveCommand.Execute(false);
        }

        private async void OnItemClicked(BarInfoGridItem item)
        {
            if (AudioPlayer.Instance.IsPlaying)
                return;

            //Add button pressed
            if(item is null)
            {
                var lastItem = (BarInfoGridItem)Items[Items.Count - 2];
                Items.Insert(Items.Count - 1, new BarInfoGridItem(lastItem.BarInfo));

                this.RaisePropertyChanged(nameof(CanUseContextMenu));

                pendingChanges = true;
            }
            else
            {
                var result = await WindowNavigation.ShowDialog<BarInfo>(
                    new TimeSignatureDialogViewModel(item.BarInfo));

                if (result.Action == DialogAction.OK)
                {
                    item.BarInfo = result.Result;

                    pendingChanges = true;
                }
            }
        }

        private void Delete(BarInfoGridItem item)
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

            this.RaisePropertyChanged(nameof(CanUseContextMenu));
        }

        private async void Save(bool notify)
        {
            var infos = Items.OfType<BarInfoGridItem>().Select(x => x.BarInfo).ToArray();
            
            try
            {
                await ClickTrackFile.Save(infos, TrackName);

                pendingChanges = false;

                if(notify)
                    MessageQueue.Enqueue($"Saved \"{TrackName}\"");
            }
            catch(Exception e)
            {
                await WindowNavigation.ShowErrorMessageDialog(e);
            }
        }

        private async void Open()
        {
            GlobalControls.Stop();

            var result = await WindowNavigation.ShowDialog<string>(
                new OpenClickTrackDialogViewModel(ClickTrackFile.Enumerate()));

            if(result.Action == DialogAction.OK)
            {
                try
                {
                    var track = await ClickTrackFile.Load(result.Result);

                    await PromptSave();

                    Items.Clear();

                    foreach (var info in track)
                        Items.Add(new BarInfoGridItem(info));

                    Items.Add(new AddButtonGridItem());

                    TrackName = result.Result;

                    Selection.RemoveSelection();
                }
                catch(Exception e)
                {
                    await WindowNavigation.ShowErrorMessageDialog(e);
                }
            }
        }

        private async void New()
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
        
        private async void Export()
        {
            var dialog = new SaveFileDialog()
            {
                Title = "Export",
                CheckPathExists = true,
                CreatePrompt = true,
                DefaultExt = ".wav",
                Filter = "WAVE | .wav"
            };

            bool? result = dialog.ShowDialog();

            if (result is null || result is false) //If no files were selected or the window was closed
                return;

            var oldValue = loop;

            loop = false;
            var track = await BuildClickTrackAsync();
            loop = oldValue;

            try
            {
                WaveFileWriter.CreateWaveFile16(dialog.FileName, track);

                MessageQueue.Enqueue($"Exported track to \"{dialog.FileName}\"");
            }
            catch(Exception e)
            {
                await WindowNavigation.ShowErrorMessageDialog(e);
            }
        }

        private void Select(BarInfoGridItem item)
        {
            if (Selection.IsSelecting)
                EndSelection(item);
            else
                BeginSelection(item);
        }

        private void BeginSelection(BarInfoGridItem item)
        {
            if (Items.Count <= 2)
                return;

            ClearSelection();

            item.IsSelected = true;

            Selection.Begin(Items.IndexOf(item));
        }

        private void EndSelection(BarInfoGridItem item)
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

        private void ClearSelection()
        {
            ApplySelection(false, true);
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

        private async void OnMetronomePlaybackStateChanged(MetronomePlaybackStateChanged msg)
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
                        var settings = await UserSettings.Get();
                        var bar = settings.ClickSettings;

                        var first = (BarInfoGridItem)Items.First();

                        timer.Interval =
                            new BarInfo(first.BarInfo.Tempo, bar.PrecountBarBeats, bar.PrecountBarNoteLength)
                            .GetInterval(SelectedPlaybackSpeed.Value) * bar.PrecountBarBeats;
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

        private void UpdateCurrentBar()
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

            Application.Current.Dispatcher.Invoke(() => ScrollToBar(currentItemIndex));

            if (currentItemIndex == lastItemIndex)
                currentItemIndex = firstItemIndex;
            else
                currentItemIndex++;

            timer.Interval = current.BarInfo.GetInterval(SelectedPlaybackSpeed.Value) * current.BarInfo.Beats;
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
