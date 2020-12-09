using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Proggy.Core
{
    public static class ClickTrackBuilder
    {
        public const int SoundDurationMs = 20;
        public static ISampleProvider BuildSinglePulse(in BarInfo info, ClickSettings settings)
        {
            var track = BuildBar(info, settings);
            return new LoopingSampleProvider(CachedSound.FromSampleProvider(track));
        }

        public static async Task<ISampleProvider> BuildClickTrackAsync(
            BarInfo[] infos,
            ClickSettings settings,
            bool precount,
            bool loop)
        {
            return await Task.Run(() => 
            {
                var providers = new ISampleProvider[infos.Length];

                for (var i = 0; i < infos.Length; i++)
                    providers[i] = CachedSound.FromSampleProvider(BuildBar(infos[i], settings));

                var track = new ConcatenatingSampleProvider(providers);

                ISampleProvider precountMeasure = null;

                if (precount)
                {
                    precountMeasure = BuildBar(
                        new BarInfo(infos[0].Tempo,
                                    settings.PrecountBarBeats,
                                    settings.PrecountBarNoteLength),
                        settings);
                }

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

        private static ISampleProvider BuildBar(in BarInfo info, ClickSettings settings)
        {
            var providers = new ISampleProvider[info.Beats];

            var silenceInterval = TimeSpan.FromMilliseconds(info.Interval - SoundDurationMs);
            
            for (var i = 0; i < info.Beats; i++)
            {
                var click = new SignalGenerator()
                {
                    Gain = 1,
                    Frequency = i > 0 ? settings.ClickFreq : settings.AccentClickFreq,
                    Type = settings.WaveType
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
