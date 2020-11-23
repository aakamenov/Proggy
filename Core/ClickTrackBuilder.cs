using System;
using System.Collections.Generic;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Proggy.Core
{
    public abstract class ClickTrackBuilder
    {
        protected const int SoundDurationMs = 10;

        public abstract ISampleProvider Build(IList<BarInfo> infos, bool precount, bool loop);

        protected ISampleProvider BuildSinglePulse(BarInfo info)
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
