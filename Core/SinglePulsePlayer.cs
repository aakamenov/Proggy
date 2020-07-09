using System;
using System.Reactive.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Proggy.Core
{
    public class SinglePulsePlayer : IClickPlayer
    {
        private bool isPlaying;
        private IDisposable clickLoop;

        public void Play(int interval, short beatsPerMeasure)
        {          
            short beatsElapsed = beatsPerMeasure;
            
            clickLoop = Observable.Interval(TimeSpan.FromMilliseconds(interval))
                .DoWhile(() => isPlaying)
                .Subscribe((_) => 
                {
                    double frequency;

                    if (beatsElapsed == beatsPerMeasure)
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
