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

        private string selectedMode;
        private readonly IClickPlayer player;

        public GlobalControlsViewModel(IClickPlayer player)
        {
            SelectedMode = modes[0];
            this.player = player;
        }

        public void Start()
        {
            player.Play(500, 4);
        }

        public void Stop()
        {
            player.Stop();
        }
    }
}
