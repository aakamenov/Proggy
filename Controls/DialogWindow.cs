using System;
using Avalonia.Controls;
using Proggy.Infrastructure;

namespace Proggy.Controls
{
    public class DialogWindow : Window
    {
        public DialogWindow()
        {
            DataContextChanged += OnDataContextChanged;          
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            if (DataContext is IDialog dialog)
            {
                dialog.Close = new Action(() =>
                {
                    dialog.IsConfirm = true;
                    Close();
                });
            }
        }
    }
}
