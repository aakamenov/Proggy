using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Akavache;
using Proggy.ViewModels;

namespace Proggy.Infrastructure
{
    public static class WindowNavigation
    {
        public static async Task<TViewModel> ShowDialogAsync<TViewModel>(Func<TViewModel> builder) where TViewModel : ViewModelBase
        {
            var vm = builder();

            var viewName = vm.GetType().Name.Replace("ViewModel", string.Empty);

            Type windowType = null;

            try
            {
                windowType = await BlobCache.InMemory.GetObject<Type>(viewName);
            }
            catch (KeyNotFoundException)
            {
                windowType = vm.GetType().Assembly.GetTypes().FirstOrDefault(x => x.Name == viewName);
#if DEBUG
                if (windowType is null)
                    throw new InvalidOperationException($"Couldn't locate view with name: {windowType.FullName}");
#endif

                await BlobCache.InMemory.InsertObject(viewName, windowType);
            }

            var window = Activator.CreateInstance(windowType) as Window;
            window.DataContext = vm;
            
            var lifeTime = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            await window.ShowDialog(lifeTime.MainWindow);
            
            return vm;
        }

        public static async Task ShowErrorMessageAsync(Exception e)
        {
            var message = string.Empty;
            
            if (e is IOException || e is UnauthorizedAccessException)
                message = e.Message;
            else
                message = "An unknown error occurred.";

            await ShowDialogAsync(() => 
            {
                return new AlertDialogViewModel(message, "Error");
            });
        }

        public static async Task<AlertDialogViewModel> PromptAsync(string message, string title = "Prompt")
        {
            return await ShowDialogAsync(() => 
            {
                return new AlertDialogViewModel(message, title: title, isYesNo: true);
            });
        }
    }
}
