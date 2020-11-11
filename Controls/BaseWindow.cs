using System;
using Avalonia.Controls;
using Proggy.Infrastructure;

namespace Proggy.Controls
{
    public class BaseWindow : Window
    {
        public BaseWindow()
        {
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            if (DataContext is IDialog dialog)
            {
                dialog.Close = new Action(() =>
                {
                    dialog.WasClosedFromView = true;
                    Close();
                });
            }
        }
    }
}
