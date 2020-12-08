using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Proggy.ViewModels;

namespace Proggy.Views
{
    public class AdvancedModeView : UserControl
    {
        private readonly ItemsRepeater items;

        public AdvancedModeView()
        {
            InitializeComponent();

            items = this.FindControl<ItemsRepeater>("Items");

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            var vm = (AdvancedModeViewModel)DataContext;
            vm.ScrollToBar = ScrollTo;
            
        }

        private void ScrollTo(int childIndex)
        {
            var child = items.Children[childIndex];
            child.BringIntoView();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
