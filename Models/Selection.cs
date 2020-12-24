using System;
using ReactiveUI;

namespace Proggy.Models
{
    public class Selection : ReactiveObject
    {
        public class SelectionRange
        {
            public int Start { get; set; }
            public int End { get; set; }
        }

        public SelectionRange Range
        {
            get => range;
            set => this.RaiseAndSetIfChanged(ref range, value);
        }

        public bool HasSelection => range != null && isSelecting == false;

        public bool IsSelecting
        {
            get => isSelecting;
            set => this.RaiseAndSetIfChanged(ref isSelecting, value);
        }

        private bool isSelecting;
        private SelectionRange range;

        public void Begin(int index)
        {
            range = new SelectionRange()
            {
                Start = index
            };

            IsSelecting = true;
        }

        public void Finalize(int index)
        {
#if DEBUG
            if (IsSelecting == false)
                throw new InvalidOperationException($"Must call {nameof(Begin)} first.");
#endif

            range.End = index;

            if (range.Start > range.End)
            {
                var swap = range.Start;
                range.Start = range.End;
                range.End = swap;
            }

            IsSelecting = false;
        }

        public void RemoveSelection()
        {
            range = null;
            IsSelecting = false;
        }

        public bool Contains(int index)
        {
            if (!HasSelection)
                return false;

            return index >= range.Start && index <= range.End;
        }
    }
}
