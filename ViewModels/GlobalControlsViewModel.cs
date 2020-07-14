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

        private string playButtonText;
        private string selectedMode;
        private readonly IClickPlayer player;

        public GlobalControlsViewModel(IClickPlayer player)
        {
            SelectedMode = modes[0];
            this.player = player;
            playButtonText = "Play";
        }

        public void Toggle()
        {
            if (player.IsPlaying)
            {
                player.Stop();
                PlayButtonText = "Play";
            }
            else
            {
                player.Play(new BarInfo[] { new BarInfo(120, 4, 4) });
                PlayButtonText = "Stop";
            }
        }

        public void Settings()
        {
            
        }
    }
}
