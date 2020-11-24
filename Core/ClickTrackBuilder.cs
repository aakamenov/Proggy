using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Proggy.Core
{
    public static class ClickTrackBuilder
    {
        private const int SoundDurationMs = 10;
        public static ISampleProvider BuildSinglePulse(BarInfo info)
        {
            var track = BuildBar(info);
            return new LoopingSampleProvider(CachedSound.FromSampleProvider(track));
        }

        public static async Task<ISampleProvider> BuildClickTrackAsync(IList<BarInfo> infos, bool precount, bool loop)
        {
            return await Task.Run(() => 
            {
                var providers = new ISampleProvider[infos.Count];

                for (var i = 0; i < infos.Count; i++)
                    providers[i] = CachedSound.FromSampleProvider(BuildBar(infos[i]));

                var track = new ConcatenatingSampleProvider(providers);

                ISampleProvider precountMeasure = null;

                if (precount)
                    precountMeasure = BuildBar(new BarInfo(infos[0].Tempo, 4, 4));

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
            });
        }

        private static ISampleProvider BuildBar(BarInfo info)
        {
            var providers = new ISampleProvider[info.Beats];

            var silenceInterval = TimeSpan.FromMilliseconds(info.Interval - SoundDurationMs);
            
            for (var i = 0; i < info.Beats; i++)
            {
                var click = new SignalGenerator()
                {
                    Gain = 0.2,
                    Frequency = i > 0 ? 2000 : 4000,
                    Type = SignalGeneratorType.Sin
                }.Take(TimeSpan.FromMilliseconds(SoundDurationMs));

                providers[i] = new OffsetSampleProvider(click)
                {
                    LeadOut = silenceInterval
                };
            }

            return new ConcatenatingSampleProvider(providers);
        }
    }
}
