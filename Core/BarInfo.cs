namespace Proggy.Core
{
    public struct BarInfo
    {
        public byte Beats { get; }
        public byte NoteLength { get; }
        public short Tempo { get; }
        public int Interval 
        { 
            get
            {
                if (!interval.HasValue)
                    CalculateInterval();

                return interval.Value;
            }
        }

        private int? interval;

        public BarInfo(short tempo, byte beats, byte noteLength)
        {
            NoteLength = noteLength;
            Tempo = tempo;
            Beats = beats;
            interval = null;
        }

        private void CalculateInterval()
        {
            interval = 60000 / Tempo;

            if (NoteLength != 4)
            {
                if (NoteLength > 4)
                    interval /= NoteLength / 4;
                else
                    interval *= NoteLength;
            }
        }
    }
}
