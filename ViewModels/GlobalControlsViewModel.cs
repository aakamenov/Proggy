using System;
using System.Linq;
using System.Threading.Tasks;
using Proggy.Core;
using Proggy.Infrastructure;
using Proggy.Infrastructure.Events;
using ReactiveUI;
using Proggy.ViewModels.CollectionItems;
using NAudio.Wave;

namespace Proggy.ViewModels
{
    public class GlobalControlsViewModel : ViewModelBase
    {
        public ListItem<MetronomeMode>[] Modes { get; }
        public ListItem<MetronomeMode> SelectedMode
        {
            get => selectedMode;
            set =>this.RaiseAndSetIfChanged(ref selectedMode, value);
        }

        public bool CanPlay
        {
            get => canPlay;
            set => this.RaiseAndSetIfChanged(ref canPlay, value);
        }

        public string PlayButtonText
        {
            get => playButtonText;
            set => this.RaiseAndSetIfChanged(ref playButtonText, value);
        }

        private string playButtonText;
        private bool canPlay;
        private ListItem<MetronomeMode> selectedMode;
        private readonly Func<Task<ISampleProvider>> buildClickTrack;

        public GlobalControlsViewModel(Func<Task<ISampleProvider>> buildClickTrack, MetronomeMode selectedMode)
        {
            this.buildClickTrack = buildClickTrack;

            Modes = new ListItem<MetronomeMode>[] 
            {
                new ListItem<MetronomeMode>("Basic", MetronomeMode.Basic),
                new ListItem<MetronomeMode>("Advanced", MetronomeMode.Advanced)
            };
            SelectedMode = Modes.First(x => selectedMode == x.Value);

            playButtonText = "Play";
            canPlay = true;

            this.ObservableForProperty(x => x.SelectedMode)
                .Subscribe(x => MessageBus.Current.SendMessage(new ModeChanged(x.Value.Value)));

            AudioPlayer.Instance.PlaybackStopped += OnPlaybackStopped;
        }

        public async void Toggle()
        {
            if (AudioPlayer.Instance.IsPlaying)
            {
                AudioPlayer.Instance.Stop();
                PlayButtonText = "Play";
            }
            else
            {
                CanPlay = false;
                AudioPlayer.Instance.PlaySound(await buildClickTrack());
                CanPlay = true;

                PlayButtonText = "Stop";
            }
        }

        public void Settings()
        {
            
        }

        private void OnPlaybackStopped(object sender, EventArgs e)
        {
            PlayButtonText = "Play";
        }

        ~GlobalControlsViewModel()
        {
            AudioPlayer.Instance.PlaybackStopped -= OnPlaybackStopped;
        }
    }
}
