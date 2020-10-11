using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Proggy.Core;
using ReactiveUI;

namespace Proggy.ViewModels
{
    public class GlobalControlsViewModel : ViewModelBase
    {
        private readonly string[] modes = { "Basic", "Advanced" };

        public string[] Modes => modes;
        public string SelectedMode
        {
            get => selectedMode;
            set => this.RaiseAndSetIfChanged(ref selectedMode, value);
        }

        public string PlayButtonText
        {
            get => playButtonText;
            set => this.RaiseAndSetIfChanged(ref playButtonText, value);
        }

        public IList<BarInfo> BarInfo => trackBuilder.BarInfo;

        private string playButtonText;
        private string selectedMode;
        private readonly IClickTrackBuilder trackBuilder;

        public GlobalControlsViewModel(IClickTrackBuilder trackBuilder)
        {
            SelectedMode = modes[0];
            this.trackBuilder = trackBuilder;
            playButtonText = "Play";
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
                AudioPlayer.Instance.PlaySound(trackBuilder.Build());
                PlayButtonText = "Stop";
            }
        }

        public void Settings()
        {
            
        }
    }
}
