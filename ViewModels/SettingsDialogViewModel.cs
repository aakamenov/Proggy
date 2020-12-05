using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ReactiveUI;
using Proggy.Core;
using Proggy.Models;
using Proggy.ViewModels.CollectionItems;

namespace Proggy.ViewModels
{
    public class SettingsDialogViewModel : ViewModelBase
    {
        public UserSettings UserSettings { get; }
        public ListItem<SignalGeneratorType>[] WaveTypes { get; }
        public ListItem<SignalGeneratorType> SelectedWaveType
        {
            get => selectedWaveType;
            set => this.RaiseAndSetIfChanged(ref selectedWaveType, value);
        }

        public double AccentClickFreq
        {
            get => accentClickFreq;
            set => this.RaiseAndSetIfChanged(ref accentClickFreq, value);
        }

        public double ClickFreq
        {
            get => clickFreq;
            set => this.RaiseAndSetIfChanged(ref clickFreq, value);
        }

        public string AccentClickFreqText => Math.Round(accentClickFreq).ToString();
        public string ClickFreqText => Math.Round(clickFreq).ToString();

        private double accentClickFreq;
        private double clickFreq;
        private ListItem<SignalGeneratorType> selectedWaveType;

        public SettingsDialogViewModel(UserSettings settings)
        {
            UserSettings = settings;

            WaveTypes = new ListItem<SignalGeneratorType>[]
            {
                new ListItem<SignalGeneratorType>("Sine", SignalGeneratorType.Sin),
                new ListItem<SignalGeneratorType>("Saw tooth", SignalGeneratorType.SawTooth),
                new ListItem<SignalGeneratorType>("Square", SignalGeneratorType.Square),
                new ListItem<SignalGeneratorType>("Triangle", SignalGeneratorType.Triangle)
            };
            
            selectedWaveType = WaveTypes.First(x => x.Value == UserSettings.ClickSettings.WaveType);
            accentClickFreq = settings.ClickSettings.AccentClickFreq;
            clickFreq = settings.ClickSettings.ClickFreq;

            this.ObservableForProperty(x => x.SelectedWaveType)
                .Subscribe(x => UserSettings.ClickSettings.WaveType = x.Value.Value);

            this.ObservableForProperty(x => x.AccentClickFreq)
                .Subscribe(x => 
                { 
                    UserSettings.ClickSettings.AccentClickFreq = x.Value;
                    this.RaisePropertyChanged(nameof(AccentClickFreqText));
                });

            this.ObservableForProperty(x => x.ClickFreq)
                .Subscribe(x =>
                {
                    UserSettings.ClickSettings.ClickFreq = x.Value;
                    this.RaisePropertyChanged(nameof(ClickFreqText));
                });
        }

        public void TestAccentClick()
        {
            var sound = new SignalGenerator()
            {
                Gain = 1,
                Frequency = UserSettings.ClickSettings.AccentClickFreq,
                Type = UserSettings.ClickSettings.WaveType
            }.Take(TimeSpan.FromMilliseconds(ClickTrackBuilder.SoundDurationMs));

            AudioPlayer.Instance.PlaySound(sound);
        }

        public void TestClick()
        {
            var sound = new SignalGenerator()
            {
                Gain = 1,
                Frequency = UserSettings.ClickSettings.ClickFreq,
                Type = UserSettings.ClickSettings.WaveType
            }.Take(TimeSpan.FromMilliseconds(ClickTrackBuilder.SoundDurationMs));

            AudioPlayer.Instance.PlaySound(sound);
        }
    }
}
