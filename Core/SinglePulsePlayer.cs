using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Proggy.Core
{
    public class SinglePulsePlayer : IClickPlayer
    {
        private bool isPlaying;
        private IDisposable clickLoop;

        public bool IsPlaying => isPlaying;

        public void Play(IEnumerable<BarInfo> clickData)
        {
            isPlaying = true;

            var info = clickData.First();
            var beatsElapsed = info.Beats;
            
            clickLoop = Observable.Interval(TimeSpan.FromMilliseconds(info.Interval))
                .DoWhile(() => isPlaying)
                .Subscribe((_) => 
                {
                    double frequency;

                    if (beatsElapsed == info.Beats)
                    {
                        frequency = 4000;
                        beatsElapsed = 1;
                    }
                    else
                    {
                        frequency = 2000;
                        beatsElapsed++;
                    }

                    var click = new SignalGenerator()
                    {
                        Gain = 0.2,
                        Frequency = frequency,
                        Type = SignalGeneratorType.Sin
                    }.Take(TimeSpan.FromMilliseconds(10));

                    AudioPlayer.Instance.PlaySound(click);
                });          
        }

        public void Stop()
        {
            isPlaying = false;

            clickLoop.Dispose();
            clickLoop = null;
        }

        ~SinglePulsePlayer()
        {
            if(clickLoop != null)
                clickLoop.Dispose();
        }
    }
}
