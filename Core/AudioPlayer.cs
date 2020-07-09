using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;


namespace Proggy.Core
{
    public class AudioPlayer : IDisposable
    {
        public static AudioPlayer Instance => instance;

        public float Volume
        {
            get => outputDevice.Volume;
            set
            {
#if DEBUG
                if (value < 0 || value > 1.0f)
                    throw new InvalidOperationException("Volume value must be between 0f and 1.0f.");
#endif
                outputDevice.Volume = value;
            }
        }

        private static readonly AudioPlayer instance;

        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;

        private AudioPlayer()
        {
            const int sampleRate = 44100;
            const int channelCount = 2;

            outputDevice = new WaveOutEvent();

            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));

            outputDevice.Init(mixer);
        }

        static AudioPlayer()
        {
            instance = new AudioPlayer();
        }

        public void PlaySound(ISampleProvider input)
        {
            AddMixerInput(input);
            outputDevice.Play();
        }

        public void Dispose()
        {
            outputDevice.Dispose();
        }

        private void AddMixerInput(ISampleProvider input)
        {
            mixer.AddMixerInput(ConvertChannelCount(input));
        }

        private ISampleProvider ConvertChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
                return input;

            if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
                return new MonoToStereoSampleProvider(input);

            throw new NotImplementedException("Channel count is more that 2");
        }
    }
}
