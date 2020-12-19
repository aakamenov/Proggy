using System;
using Proggy.Infrastructure;

namespace Proggy.ViewModels
{
    public class AlertDialogViewModel : ViewModelBase, IDialog
    {
        public string Message { get; }
        public string Title { get; }
        public Action Close { get; set; }
        public bool IsConfirm { get; set; }

        public AlertDialogViewModel(string message, string title = "Alert")
        {
            Message = message;
            Title = title;
        }

        public void Cancel()
        {
            Close?.Invoke();
        }
    }
}
