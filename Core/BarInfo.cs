namespace Proggy.Core
{
    public struct BarInfo
    {
        public byte Beats { get; }
        public bool[] Accents
        {
            get
            {
                if(accents is null)
                {
                    accents = new bool[Beats];
                    accents[0] = true;
                }

                return accents;
            }
        }
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
        private bool[] accents;

        public BarInfo(short tempo, byte beats, byte noteLength)
        {
            NoteLength = noteLength;
            Tempo = tempo;
            Beats = beats;

            accents = null;
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
