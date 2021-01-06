using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Timers;
using ReactiveUI;
using Proggy.Infrastructure;
using Proggy.Infrastructure.Commands;

namespace Proggy.ViewModels.Dialogs
{
    public class OpenClickTrackDialogViewModel : BaseDialogViewModel
    {
        public ObservableCollection<string> Tracks { get; }

        public string SelectedTrack
        {
            get => selectedTrack;
            set => this.RaiseAndSetIfChanged(ref selectedTrack, value);
        }

        public bool CanOpen
        {
            get => canOpen;
            set => this.RaiseAndSetIfChanged(ref canOpen, value);
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set => this.RaiseAndSetIfChanged(ref errorMessage, value);
        }

        public Command DeleteCommand { get; }
        public Command OpenFolderCommand { get; }

        private bool canOpen;
        private string selectedTrack;
        private string errorMessage;

        public OpenClickTrackDialogViewModel(string[] tracks)
        {
            Tracks = new ObservableCollection<string>(tracks);

            if (tracks.Length > 0)
            {
                selectedTrack = tracks[0];
                canOpen = true;
            }

            Tracks.CollectionChanged += OnTracksCollectionChanged;

            DeleteCommand = new Command(Delete);
            OpenFolderCommand = new Command(OpenTracksFolder);
        }

        protected override void Ok()
        {
            Close.Invoke(new DialogResult<string>(DialogAction.OK, SelectedTrack));
        }

        protected override void Cancel()
        {
            Close.Invoke(new DialogResult<string>(DialogAction.Cancel, null));
        }

        private void OpenTracksFolder()
        {
            Process.Start("explorer.exe", ClickTrackFile.FolderPath);
        }

        private void Delete()
        {
            try
            {
                ClickTrackFile.Delete(SelectedTrack);

                Tracks.Remove(SelectedTrack);
                SelectedTrack = Tracks.LastOrDefault();
            }
            catch(Exception e)
            {
                ErrorMessage = e.Message;

                var timer = new Timer(4000)
                {
                    AutoReset = false
                };

                timer.Elapsed += (s, e) => 
                {
                    ErrorMessage = null;
                    timer.Stop();
                    timer.Dispose();
                };
                
                timer.Start();
            }
        }

        private void OnTracksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (Tracks.Count == 0)
                    CanOpen = false;
            }
        }
    }
}
