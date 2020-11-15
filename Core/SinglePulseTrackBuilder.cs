using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;

namespace Proggy.Core
{
    public class SinglePulseTrackBuilder : ClickTrackBuilder
    {
        public override ISampleProvider Build(IList<BarInfo> infos, bool precount, bool loop)
        {
            var track = BuildSinglePulse(infos.First());      
            return new LoopingSampleProvider(CachedSound.FromSampleProvider(track));
        }
    }
}
