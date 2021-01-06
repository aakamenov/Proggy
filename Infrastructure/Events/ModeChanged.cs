namespace Proggy.Infrastructure.Events
{
    public enum MetronomeMode
    {
        Basic,
        Advanced,
    }

    public class ModeChanged
    {
        public MetronomeMode Mode { get; }

        public ModeChanged(MetronomeMode mode)
        {
            Mode = mode;
        }
    }
}
