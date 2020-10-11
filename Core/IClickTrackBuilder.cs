using NAudio.Wave;
using System.Collections.Generic;

namespace Proggy.Core
{
    public interface IClickTrackBuilder
    {
        ISampleProvider Build();
        IList<BarInfo> BarInfo { get; set; }
    }
}
