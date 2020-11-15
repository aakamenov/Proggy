using System;
using System.Collections.Generic;
using System.Linq;
using Proggy.Core;
using Proggy.Infrastructure;
using Proggy.Infrastructure.Events;
using ReactiveUI;
using Proggy.ViewModels.CollectionItems;

namespace Proggy.ViewModels
{
    public class GlobalControlsViewModel : ViewModelBase
    {
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

        public ListItem<MetronomeMode>[] Modes { get; }
        public ListItem<MetronomeMode> SelectedMode
        {
            get => selectedMode;
            set =>this.RaiseAndSetIfChanged(ref selectedMode, value);
        }

        public string PlayButtonText
        {
            get => playButtonText;
            set => this.RaiseAndSetIfChanged(ref playButtonText, value);
        }

        public IList<BarInfo> ClickTrack => clickTrack;

        private string playButtonText;
        private bool loop;
        private bool precount;
        private List<BarInfo> clickTrack;
        private ListItem<MetronomeMode> selectedMode;
        private readonly ClickTrackBuilder trackBuilder;

        public GlobalControlsViewModel(ClickTrackBuilder trackBuilder, MetronomeMode selectedMode)
        {
            Modes = new ListItem<MetronomeMode>[] 
            {
                new ListItem<MetronomeMode>("Basic", MetronomeMode.Basic),
                new ListItem<MetronomeMode>("Advanced", MetronomeMode.Advanced)
            };
            SelectedMode = Modes.First(x => selectedMode == x.Value);

            clickTrack = new List<BarInfo>();

            this.trackBuilder = trackBuilder;
            playButtonText = "Play";

            this.ObservableForProperty(x => x.SelectedMode)
                .Subscribe(x => MessageBus.Current.SendMessage(new ModeChanged(x.Value.Value)));

            AudioPlayer.Instance.PlaybackStopped += OnPlaybackStopped;
        }

        public void Toggle()
        {
            if (AudioPlayer.Instance.IsPlaying)
            {
                AudioPlayer.Instance.Stop();
                PlayButtonText = "Play";
            }
            else
            {
                AudioPlayer.Instance.PlaySound(trackBuilder.Build(clickTrack, precount, loop));
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
