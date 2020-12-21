using System;

namespace Proggy.Models
{
    public class Selection
    {
        public int Start { get; private set; }
        public int End { get; private set; }
        public bool IsSelecting { get; private set; }

        public void Begin(int index)
        {
            Start = index;
            IsSelecting = true;
        }

        public void Finalize(int index)
        {
            if (IsSelecting == false)
                throw new InvalidOperationException($"Must call {nameof(Begin)} first.");

            End = index;

            if (Start > End)
            {
                var swap = Start;
                Start = End;
                End = swap;
            }

            IsSelecting = false;
        }
    }
}
