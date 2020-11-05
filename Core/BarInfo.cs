namespace Proggy.Core
{
    public class BarInfo
    {
        public int Interval { get; }
        public short Beats { get; }
        public short NoteLength { get; }
        public short Tempo { get; }

        public BarInfo(short tempo, short beats, short noteLength)
        {
            NoteLength = noteLength;
            Tempo = tempo;
            Beats = beats;
            Interval = 60000 / tempo;

            if(noteLength != 4)
            {
                if (noteLength > 4)
                    Interval /= noteLength / 4;
                else
                    Interval *= noteLength;
            }
        }
    }
}
