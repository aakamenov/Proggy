using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Proggy.Core;
using Proggy.Infrastructure;
using Proggy.Infrastructure.Events;
using Proggy.Models;
using ReactiveUI;

namespace Proggy.ViewModels
{
    public class GlobalControlsViewModel : ViewModelBase
    {
        public UiCollectionItem<MetronomeMode>[] Modes { get; }
        public UiCollectionItem<MetronomeMode> SelectedMode
        {
            get => selectedMode;
            set =>this.RaiseAndSetIfChanged(ref selectedMode, value);
        }

        public string PlayButtonText
        {
            get => playButtonText;
            set => this.RaiseAndSetIfChanged(ref playButtonText, value);
        }

        public IList<BarInfo> BarInfo => trackBuilder.BarInfo;

        private string playButtonText;
        private UiCollectionItem<MetronomeMode> selectedMode;
        private readonly IClickTrackBuilder trackBuilder;

        public GlobalControlsViewModel(IClickTrackBuilder trackBuilder, MetronomeMode selectedMode)
        {
            Modes = new UiCollectionItem<MetronomeMode>[] 
            {
                new UiCollectionItem<MetronomeMode>("Basic", MetronomeMode.Basic),
                new UiCollectionItem<MetronomeMode>("Advanced", MetronomeMode.Advanced)
            };
            SelectedMode = Modes.First(x => selectedMode == x.Value);

            this.trackBuilder = trackBuilder;
            playButtonText = "Play";

            this.ObservableForProperty(x => x.SelectedMode)
                .Subscribe(x => MessageBus.Current.SendMessage(new ModeChanged(x.Value.Value)));
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
