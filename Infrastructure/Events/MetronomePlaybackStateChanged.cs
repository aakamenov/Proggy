namespace Proggy.Infrastructure.Events
{
    public enum MetronomePlaybackState
    {
        Playing,
        Stopped
    }
    public class MetronomePlaybackStateChanged
    {
        public MetronomePlaybackState State { get; }

        public MetronomePlaybackStateChanged(MetronomePlaybackState state)
        {
            State = state;
        }
    }
}
