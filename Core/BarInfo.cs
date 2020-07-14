namespace Proggy.Core
{
    public struct BarInfo
    {
        public int Interval { get; }
        public short Beats { get; }

        public BarInfo(int tempo, short beats, short noteLength)
        {
            Beats = beats;
            Interval = 60000 / tempo;

            if(noteLength != 4)
            {
                if (noteLength > 4)
                    Interval /= noteLength;
                else
                    Interval *= noteLength;
            }
        }
    }
}
