using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using Akavache;
using Proggy.Views;
using Proggy.Models;

namespace Proggy
{
    public class App : Application
    {
        private PaletteHelper paletteHelper;

        public override void Initialize()
        {
            BlobCache.ApplicationName = "Proggy";

            AvaloniaXamlLoader.Load(this);

            paletteHelper = new PaletteHelper();
            paletteHelper.GetThemeManager().ThemeChanged += OnThemeChanged;

            SetThemeColors(paletteHelper.GetTheme());
        }

        public async override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            var settings = await UserSettings.Get();
            paletteHelper.SetTheme(settings.Theme);

            base.OnFrameworkInitializationCompleted();
        }
        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            SetThemeColors(e.NewTheme);
        }

        private void SetThemeColors(ITheme theme)
        {
            Color primaryColor;
            Color secondaryColor;

            if(theme.GetBaseTheme() == BaseThemeMode.Dark)
            {
                primaryColor = theme.PrimaryDark.Color;
                secondaryColor = theme.SecondaryDark.Color;
            }
            else
            {
                primaryColor = theme.PrimaryLight.Color;
                secondaryColor = theme.SecondaryLight.Color;
            }

            Resources["PrimaryColor"] = primaryColor;
            Resources["SecondaryColor"] = secondaryColor;
        }
    }
}
