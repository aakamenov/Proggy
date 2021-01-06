namespace Proggy.ViewModels.Dialogs
{
    public class AlertDialogViewModel : BaseDialogViewModel
    {
        public string Message { get; }
        public bool IsYesNo { get; }
        public string OkButtonText { get; }
        public string CancelButtonText { get; }

        public AlertDialogViewModel(
            string message, 
            bool isYesNo = false)
        {
            Message = message;
            IsYesNo = isYesNo;

            if(isYesNo)
            {
                OkButtonText = "Yes";
                CancelButtonText = "No";
            }
            else
                OkButtonText = "OK";
        }

        protected override void Ok()
        {
            Close.Invoke(DialogResult.Ok);
        }

        protected override void Cancel()
        {
            Close.Invoke(DialogResult.Cancel);
        }
    }
}
