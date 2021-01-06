using System.Windows;
using System.Windows.Media;
using Proggy.Models;
using Proggy.Core;
using Akavache;
using MaterialDesignThemes.Wpf;
using MaterialDesignColors;


namespace Proggy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private PaletteHelper paletteHelper;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            BlobCache.ApplicationName = Constants.AppName;

            paletteHelper = new PaletteHelper();
            paletteHelper.GetThemeManager().ThemeChanged += OnThemeChanged;

            SetThemeColors(paletteHelper.GetTheme());

            var settings = await UserSettings.Get();
            paletteHelper.SetTheme(settings.Theme);
            
            Resources["ErrorTextColor"] = new SolidColorBrush(SwatchHelper.Lookup[MaterialDesignColor.Red]);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            AudioPlayer.Instance.Dispose();
            await BlobCache.Shutdown();
        }

        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            SetThemeColors(e.NewTheme);
        }

        private void SetThemeColors(ITheme theme)
        {
            Color primaryColor;
            Color accentColor;

            if (theme.GetBaseTheme() == BaseTheme.Dark)
            {
                primaryColor = theme.PrimaryDark.Color;
                accentColor = theme.SecondaryDark.Color;
            }
            else
            {
                primaryColor = theme.PrimaryLight.Color;
                accentColor = theme.SecondaryLight.Color;
            }

            Resources["PrimaryColor"] = new SolidColorBrush(primaryColor);
            Resources["AccentColor"] = new SolidColorBrush(accentColor);
        }
    }
}
