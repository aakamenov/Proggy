using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Proggy.ViewModels;
using Proggy.Views;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;

namespace Proggy
{
    public class App : Application
    {
        private PaletteHelper paletteHelper;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            paletteHelper = new PaletteHelper();
            paletteHelper.GetThemeManager().ThemeChanged += OnThemeChanged;

            SetThemeColors(paletteHelper.GetTheme());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            SetThemeColors(e.NewTheme);
        }

        private void SetThemeColors(ITheme theme)
        {
            var primaryColor = theme.GetBaseTheme() switch
            {
                BaseThemeMode.Dark => theme.PrimaryDark,
                BaseThemeMode.Light => theme.PrimaryLight,
                BaseThemeMode.Inherit => theme.PrimaryMid
            };

            var secondaryColor = theme.GetBaseTheme() switch
            {
                BaseThemeMode.Dark => theme.SecondaryDark,
                BaseThemeMode.Light => theme.SecondaryLight,
                BaseThemeMode.Inherit => theme.SecondaryMid
            };

            Resources["PrimaryColor"] = primaryColor.Color;
            Resources["SecondaryColor"] = secondaryColor.Color;
        }
    }
}
