using System;
using System.Reactive.Linq;
using System.Linq;
using System.Windows.Media;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ReactiveUI;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Proggy.Core;
using Proggy.Models;
using Proggy.ViewModels.CollectionItems;
using Proggy.Infrastructure.Commands;

namespace Proggy.ViewModels
{
    public class SettingsWindowViewModel : ViewModelBase
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

        public BaseTheme ThemeMode
        {
            get => themeMode;
            set => this.RaiseAndSetIfChanged(ref themeMode, value);
        }

        public Color[] PrimaryColors { get; private set; }
        public Color[] SecondaryColors { get; private set; }

        public Color PrimaryColor
        {
            get => primaryColor;
            set => this.RaiseAndSetIfChanged(ref primaryColor, value);
        }

        public Color SecondaryColor
        {
            get => secondaryColor;
            set => this.RaiseAndSetIfChanged(ref secondaryColor, value);
        }

        public TimeSignatureControlsViewModel TimeSignatureControls { get; }

        public Command TestClickCommand { get; }
        public Command TestAccentClickCommand { get; }
        public Command<BaseTheme> ChangeThemeCommand { get; }

        private readonly PaletteHelper paletteHelper;

        private double accentClickFreq;
        private double clickFreq;
        private ListItem<SignalGeneratorType> selectedWaveType;
        private BaseTheme themeMode;
        private Color primaryColor;
        private Color secondaryColor;

        public SettingsWindowViewModel(UserSettings settings)
        {
            UserSettings = settings;

            paletteHelper = new PaletteHelper();

            TimeSignatureControls = new TimeSignatureControlsViewModel()
            {
                SelectedBeats = settings.ClickSettings.PrecountBarBeats,
                SelectedNoteLength = settings.ClickSettings.PrecountBarNoteLength
            };

            WaveTypes = new ListItem<SignalGeneratorType>[]
            {
                new ListItem<SignalGeneratorType>("Sine", SignalGeneratorType.Sin),
                new ListItem<SignalGeneratorType>("Saw tooth", SignalGeneratorType.SawTooth),
                new ListItem<SignalGeneratorType>("Square", SignalGeneratorType.Square),
                new ListItem<SignalGeneratorType>("Triangle", SignalGeneratorType.Triangle)
            };

            InitColors();

            primaryColor = settings.Theme.PrimaryMid.Color;
            secondaryColor = settings.Theme.SecondaryMid.Color;
            
            selectedWaveType = WaveTypes.First(x => x.Value == UserSettings.ClickSettings.WaveType);
            accentClickFreq = settings.ClickSettings.AccentClickFreq;
            clickFreq = settings.ClickSettings.ClickFreq;
            themeMode = settings.Theme.GetBaseTheme();

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

            this.ObservableForProperty(x => x.PrimaryColor)
                .Subscribe(x => ChangeTheme((theme) => 
                {
                    theme.SetPrimaryColor(x.Value);
                }));

            this.ObservableForProperty(x => x.SecondaryColor)
                .Subscribe(x => ChangeTheme((theme) =>
                {
                    theme.SetSecondaryColor(x.Value);
                }));

            this.WhenAnyValue(x => x.TimeSignatureControls.SelectedBeats,
                              x => x.TimeSignatureControls.SelectedNoteLength)
                .Subscribe(x => 
                {
                    UserSettings.ClickSettings.PrecountBarBeats = x.Item1;
                    UserSettings.ClickSettings.PrecountBarNoteLength = x.Item2;
                });

            TestClickCommand = new Command(TestClick);
            TestAccentClickCommand = new Command(TestAccentClick);
            ChangeThemeCommand = new Command<BaseTheme>(ChangeThemeMode);
        }

        private void TestAccentClick()
        {
            var sound = new SignalGenerator()
            {
                Gain = 1,
                Frequency = UserSettings.ClickSettings.AccentClickFreq,
                Type = UserSettings.ClickSettings.WaveType
            }.Take(TimeSpan.FromMilliseconds(ClickTrackBuilder.SoundDurationMs));

            AudioPlayer.Instance.PlaySound(sound);
        }

        private void TestClick()
        {
            var sound = new SignalGenerator()
            {
                Gain = 1,
                Frequency = UserSettings.ClickSettings.ClickFreq,
                Type = UserSettings.ClickSettings.WaveType
            }.Take(TimeSpan.FromMilliseconds(ClickTrackBuilder.SoundDurationMs));

            AudioPlayer.Instance.PlaySound(sound);
        }

        private void ChangeThemeMode(BaseTheme themeMode)
        {
            ThemeMode = themeMode;

            ChangeTheme((theme) => theme.SetBaseTheme(themeMode.GetBaseTheme()));        
        }

        private void ChangeTheme(Action<ITheme> change)
        {
            var theme = paletteHelper.GetTheme();

            change(theme);
            paletteHelper.SetTheme(theme);

            UserSettings.Theme = theme;
        }

        private void InitColors()
        {
            Color[] Init(Type @enum)
            {
                var values = Enum.GetValues(@enum);
                var result = new Color[values.Length];
                
                for (var i = 0; i < values.Length; i++)
                    result[i] = SwatchHelper.Lookup[(MaterialDesignColor)values.GetValue(i)];

                return result;
            }
            
            PrimaryColors = Init(typeof(PrimaryColor));
            SecondaryColors = Init(typeof(SecondaryColor));
        }
    }
}
