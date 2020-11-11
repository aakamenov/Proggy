using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Proggy.Controls;
using Proggy.ViewModels;

namespace Proggy.Infrastructure
{
    public static class WindowNavigation
    {
        public static async Task<TViewModel> NavigateAsync<TViewModel>(Func<TViewModel> builder) where TViewModel : ViewModelBase
        {
            var vm = builder();

            var viewName = vm.GetType().Name.Replace("ViewModel", string.Empty);
            var windowType = vm.GetType().Assembly.GetTypes().FirstOrDefault(x => x.Name == viewName);

            if(windowType is null)
                throw new InvalidOperationException($"Couldn't locate view with name: {windowType.FullName}");

            var window = Activator.CreateInstance(windowType) as Window;
            window.DataContext = vm;
            
            var lifeTime = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            await window.ShowDialog(lifeTime.MainWindow);

            return vm;
        }
    }
}
