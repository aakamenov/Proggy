using System;
using System.Reactive.Linq;
using ReactiveUI;
using Avalonia;
using Avalonia.Media;
using Material.Styles.Themes;
using Proggy.Core;

namespace Proggy.ViewModels.CollectionItems
{
    public class BarInfoGridItem : ClickTrackGridItem
    {
        private static IThemeManager themeManager;
        private static IBrush selectedBackgroundColor;
        private static IBrush deselectedBackgroundColor;

        static BarInfoGridItem()
        {
            themeManager = new PaletteHelper().GetThemeManager();
            themeManager.ThemeChanged += OnThemeChanged;

            SetBackgroundColors();
        }

        public bool IsSelected
        {
            get => isSelected;
            set => this.RaiseAndSetIfChanged(ref isSelected, value);
        }
        
        public IBrush BackgroundColor
        {
            get
            {
                if (isSelected)
                    return selectedBackgroundColor;

                return deselectedBackgroundColor;
            }
        }

        public BarInfo BarInfo 
        {
            get => barInfo;
            set => this.RaiseAndSetIfChanged(ref barInfo, value);
        }

        public string TimeSignature => $"{BarInfo.Beats}/{BarInfo.NoteLength}";

        private BarInfo barInfo;
        private bool isSelected;

        public BarInfoGridItem(BarInfo info)
        {
            BarInfo = info;
            this.WhenAnyValue(x => x.BarInfo).Subscribe(_ => this.RaisePropertyChanged(nameof(TimeSignature)));
            this.WhenAnyValue(x => x.IsSelected).Subscribe(_ => this.RaisePropertyChanged(nameof(BackgroundColor)));
        }

        private static void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            SetBackgroundColors();
        }

        private static void SetBackgroundColors()
        {
            selectedBackgroundColor = new SolidColorBrush((Color)Application.Current.Resources["SecondaryColor"]);
            deselectedBackgroundColor = new SolidColorBrush(Application.Current.Resources.GetTheme().CardBackground);
        }
    }
}
