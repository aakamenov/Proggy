namespace Proggy.Infrastructure.Events
{
    public class ModeChanged
    {
        public MetronomeMode Mode { get; }

        public ModeChanged(MetronomeMode mode)
        {
            Mode = mode;
        }
    }
}
