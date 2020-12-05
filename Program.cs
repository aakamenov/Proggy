using System.Threading.Tasks;
using Avalonia;
using Avalonia.ReactiveUI;
using Proggy.Core;
using Akavache;

namespace Proggy
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static async Task Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            AudioPlayer.Instance.Dispose();
            await BlobCache.Shutdown();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();
    }
}
