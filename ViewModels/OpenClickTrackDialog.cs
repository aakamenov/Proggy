using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ReactiveUI;
using Proggy.Infrastructure;

namespace Proggy.ViewModels
{
    public class OpenClickTrackDialog : ViewModelBase, IDialog
    {
        public ObservableCollection<string> Tracks { get; }
        public string SelectedTrack
        {
            get => selectedTrack;
            set => this.RaiseAndSetIfChanged(ref selectedTrack, value);
        }

        public Action Close { get; set; }
        public bool IsConfirm { get; set; }

        public bool CanOpen
        {
            get => canOpen;
            set => this.RaiseAndSetIfChanged(ref canOpen, value);
        }

        private bool canOpen;
        private string selectedTrack;

        public OpenClickTrackDialog(string[] tracks)
        {
            Tracks = new ObservableCollection<string>(tracks);

            if (tracks.Length > 0)
            {
                selectedTrack = tracks[0];
                canOpen = true;
            }

            Tracks.CollectionChanged += OnTracksCollectionChanged;
        }

        private void OnTracksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (Tracks.Count == 0)
                    CanOpen = false;
            }
        }

        public void Open()
        {
            Close?.Invoke();
        }

        public void OpenTracksFolder()
        {
            Process.Start("explorer.exe", ClickTrackFile.FolderPath);
        }

        public async void Delete()
        {
            try
            {
                ClickTrackFile.Delete(SelectedTrack);

                Tracks.Remove(SelectedTrack);
                SelectedTrack = Tracks.LastOrDefault();
            }
            catch(Exception e)
            {
                await WindowNavigation.ShowErrorMessageAsync(e);
            }
        }
    }
}
