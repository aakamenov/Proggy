using System.Collections.Generic;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Proggy.Core
{
    public class MultiPulseTrackBuilder : ClickTrackBuilder
    {
        public override ISampleProvider Build(IList<BarInfo> infos, bool precount, bool loop)
        {
            var providers = new List<ISampleProvider>();

            foreach (var info in infos)
                providers.Add(CachedSound.FromSampleProvider(BuildSinglePulse(info)));

            var track = new ConcatenatingSampleProvider(providers);

            ISampleProvider precountMeasure = null;

            if (precount)
                precountMeasure = BuildSinglePulse(new BarInfo(infos[0].Tempo, 4, 4));

            if (loop)
            {
                if (precountMeasure is null)
                    return new LoopingSampleProvider(CachedSound.FromSampleProvider(track));
                else
                    return precountMeasure.FollowedBy(new LoopingSampleProvider(CachedSound.FromSampleProvider(track)));
            }
            else
            {
                if (precountMeasure is null)
                    return track;
                else
                    return precountMeasure.FollowedBy(track);
            }
        }
    }
}
