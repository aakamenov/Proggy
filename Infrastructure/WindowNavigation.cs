using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Akavache;
using Proggy.ViewModels;
using Proggy.ViewModels.Dialogs;
using MaterialDesignThemes.Wpf;

namespace Proggy.Infrastructure
{
    public static class WindowNavigation
    {
        public static async Task<TViewModel> OpenWindow<TViewModel>(Func<TViewModel> builder) where TViewModel : ViewModelBase
        {
            var vm = builder();

            var windowType = await FindView(vm.GetType());

            var window = Activator.CreateInstance(windowType) as Window;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.DataContext = vm;

            window.ShowDialog();
            
            return vm;
        }

        public static async Task<DialogResult> ShowDialog(BaseDialogViewModel vm)
        {
            var result = await ShowDialogInternal(vm) as DialogResult;

            //The static method DialogHost.Close was called
            if (result is null)
                return DialogResult.Cancel;

            return result;
        }

        public static async Task<DialogResult<T>> ShowDialog<T>(BaseDialogViewModel vm)
        {
            var result = await ShowDialogInternal(vm) as DialogResult<T>;

            //The static method DialogHost.Close was called
            if (result is null)
                return new DialogResult<T>(DialogAction.Cancel, default);

            return result;
        }

        public static async Task<DialogResult> ShowErrorMessageDialog(Exception e)
        {
            string message;

            if (e is IOException || e is UnauthorizedAccessException)
                message = e.Message;
            else
                message = "An unknown error occurred.";

            return await ShowDialog(new AlertDialogViewModel(message));
        }

        private static async Task<object> ShowDialogInternal(BaseDialogViewModel vm)
        {
            var viewType = await FindView(vm.GetType());

            var view = Activator.CreateInstance(viewType) as UserControl;
            view.DataContext = vm;

            return await DialogHost.Show(view, openedEventHandler: (sender, e) => 
            {
                vm.Close = e.Session.Close;
            });
        }

        private static async Task<Type> FindView(Type vm)
        {
            var viewName = vm.Name.Replace("ViewModel", string.Empty);

            Type type = null;

            try
            {
                type = await BlobCache.InMemory.GetObject<Type>(viewName);
            }
            catch (KeyNotFoundException)
            {
                type = vm.Assembly.GetTypes().FirstOrDefault(x => x.Name == viewName);
#if DEBUG
                if (type is null)
                    throw new InvalidOperationException($"Couldn't locate view with name: {type.FullName}");
#endif

                await BlobCache.InMemory.InsertObject(viewName, type);
            }

            return type;
        }
    }
}
