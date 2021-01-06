using System;
using Proggy.Infrastructure.Commands;

namespace Proggy.ViewModels.Dialogs
{
    public enum DialogAction
    {
        OK = 0,
        Cancel = 1
    }

    public class DialogResult
    {
        public static readonly DialogResult Ok = new DialogResult(DialogAction.OK);
        public static readonly DialogResult Cancel = new DialogResult(DialogAction.Cancel);

        public DialogAction Action { get; }

        public DialogResult(DialogAction action)
        {
            Action = action;
        }
    }

    public class DialogResult<T> : DialogResult
    {
        public T Result { get; }

        public DialogResult(DialogAction action, T result) : base(action)
        {
            Result = result;
        }
    }

    public abstract class BaseDialogViewModel : ViewModelBase
    {
        public Command OkCommand { get; }
        public Command CancelCommand { get; }

        public Action<DialogResult> Close { get; set; }

        public BaseDialogViewModel()
        {
            OkCommand = new Command(Ok);
            CancelCommand = new Command(Cancel);
        }

        protected abstract void Ok();

        protected abstract void Cancel();
    }
}
