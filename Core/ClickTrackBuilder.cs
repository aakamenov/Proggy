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

            return track;
        }
    }
}
