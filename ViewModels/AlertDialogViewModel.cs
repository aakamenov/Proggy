using System;
using Proggy.Infrastructure;

namespace Proggy.ViewModels
{
    public class AlertDialogViewModel : ViewModelBase, IDialog
    {
        public string Message { get; }
        public string Title { get; }
        public bool IsYesNo { get; }
        public string OkButtonText { get; }
        public string CancelButtonText { get; }
        public DialogAction Result { get; private set; }

        public Action Close { get; set; }
        public bool IsConfirm { get; set; }

        public AlertDialogViewModel(string message, string title = "Alert", bool isYesNo = false)
        {
            Message = message;
            Title = title;
            IsYesNo = isYesNo;

            if(isYesNo)
            {
                OkButtonText = "Yes";
                CancelButtonText = "No";
            }
            else
                OkButtonText = "OK";
        }

        public void Ok()
        {
            Result = DialogAction.OK;
            Close?.Invoke();
        }

        public void Cancel()
        {
            Result = DialogAction.Cancel;
            Close?.Invoke();
        }
    }
}
