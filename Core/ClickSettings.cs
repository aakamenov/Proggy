using System;
using Newtonsoft.Json;
using NAudio.Wave.SampleProviders;

namespace Proggy.Core
{
    public class ClickSettings
    {
        public const byte MinPlaybackSpeedPercent = 30;

        public SignalGeneratorType WaveType { get; set; }
        public double AccentClickFreq { get; set; }
        public double ClickFreq { get; set; }
        public byte PrecountBarBeats { get; set; }
        public byte PrecountBarNoteLength { get; set; }
        [JsonIgnore]
        public float PlaybackSpeedPercent
        {
            get => playbackSpeedPercent;
            set => playbackSpeedPercent = Math.Clamp(value, MinPlaybackSpeedPercent, 100f);
        }

        private float playbackSpeedPercent;
    }
}
