using System;
using System.Collections.Generic;
using System.Text;
using Proggy.Core;

namespace Proggy.Models
{
    public class BarInfoGridItem : ClickTrackGridItem
    {
        public BarInfo BarInfo { get; }
        public string TimeSignature => $"{BarInfo.Beats}/{BarInfo.NoteLength}";
        public BarInfoGridItem(BarInfo info)
        {
            BarInfo = info;
        }
    }
}
