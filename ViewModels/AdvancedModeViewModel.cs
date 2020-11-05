using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Material.Styles.Themes;
using Proggy.Core;
using Proggy.Infrastructure;
using Proggy.Models;

namespace Proggy.ViewModels
{
    public class AdvancedModeViewModel : ViewModelBase
    {
        public ObservableCollection<ClickTrackGridItem> Items { get; }

        public GlobalControlsViewModel GlobalControls => globalControls;
        private readonly GlobalControlsViewModel globalControls;

        public AdvancedModeViewModel()
        {
            globalControls = new GlobalControlsViewModel(new SinglePulseTrackBuilder(), MetronomeMode.Advanced);
            globalControls.BarInfo.Add(new BarInfo(120, 4, 4));
            Items = new ObservableCollection<ClickTrackGridItem>();
            Items.Add(new BarInfoGridItem(globalControls.BarInfo.First()));
            Items.Add(new AddButtonGridItem());
        }
    }
}
