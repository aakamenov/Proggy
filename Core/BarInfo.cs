namespace Proggy.Core
{
    public class BarInfo
    {
        public short Beats { get; }
        public short NoteLength { get; }
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

        public BarInfo(short tempo, short beats, short noteLength)
        {
            NoteLength = noteLength;
            Tempo = tempo;
            Beats = beats;
        }

        public BarInfo DeepCopy()
        {
            var instance = new BarInfo(Tempo, Beats, NoteLength)
            {
                interval = interval
            };

            return instance;
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
