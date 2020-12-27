namespace Proggy.Core
{
    public struct BarInfo
    {
        public static readonly BarInfo Default = new BarInfo(120, 4, 4);

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

        private bool[] accents;

        public BarInfo(short tempo, byte beats, byte noteLength)
        {
            NoteLength = noteLength;
            Tempo = tempo;
            Beats = beats;

            accents = null;
        }

        public int GetInterval(float percentage = 100)
        {
            var newTempo = Tempo;

            if (percentage < 100)
                newTempo = (byte)(percentage / 100f * newTempo);

            var interval = 60000 / newTempo;

            if (NoteLength != 4)
            {
                if (NoteLength > 4)
                    interval /= NoteLength / 4;
                else
                    interval *= NoteLength;
            }

            return interval;
        }
    }
}
