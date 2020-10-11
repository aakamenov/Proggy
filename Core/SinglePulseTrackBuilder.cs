using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Proggy.Core
{
    public class SinglePulseTrackBuilder : IClickTrackBuilder
    {
        private const int SoundDurationMs = 10;

        public IList<BarInfo> BarInfo { get; set; }

        public SinglePulseTrackBuilder()
        {
            BarInfo = new List<BarInfo>();
        }

        public ISampleProvider Build()
        {
            var info = BarInfo.First();

            var track = new SignalGenerator()
            {
                Gain = 0.2,
                Frequency = 4000,
                Type = SignalGeneratorType.Sin
            }.Take(TimeSpan.FromMilliseconds(SoundDurationMs));

            var silenceInterval = TimeSpan.FromMilliseconds(info.Interval - SoundDurationMs);

            for (var i = 1; i < info.Beats; i++)
            {
                var click = new SignalGenerator()
                {
                    Gain = 0.2,
                    Frequency = 2000,
                    Type = SignalGeneratorType.Sin
                }.Take(TimeSpan.FromMilliseconds(SoundDurationMs));

                track = track.FollowedBy(silenceInterval, click);
            }

            track = track.FollowedBy(silenceInterval, new SignalGenerator()
            {
                Gain = 0,
                Frequency = 0,
                Type = SignalGeneratorType.Sin
            }.Take(TimeSpan.FromMilliseconds(SoundDurationMs)));
            
            return new LoopingSampleProvider(CachedSound.FromSampleProvider(track));
        }
    }
}
