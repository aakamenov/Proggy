using System;
using System.Threading;
using System.Diagnostics;

namespace Proggy.Infrastructure
{
    public class AccurateTimer
    {
        public int Interval { get; set; } = 100;

        private bool isRunning;
        private Thread thread;

        private readonly Action callback;
        private readonly Stopwatch stopwatch;

        public AccurateTimer(Action callback)
        {
            this.callback = callback;
            stopwatch = new Stopwatch();
        }

        public void Start()
        {
            thread = new Thread(new ThreadStart(MeasureTime))
            {
                IsBackground = true
            };

            thread.Start();
        }

        public void Stop()
        {
            isRunning = false;
            stopwatch.Reset();
        }

        private void MeasureTime()
        {
            stopwatch.Start();

            isRunning = true;

            var lastRecorded = stopwatch.ElapsedMilliseconds;

            while (isRunning)
            {
                var delta = stopwatch.ElapsedMilliseconds - lastRecorded;

                if (delta >= Interval)
                {
                    callback();

                    lastRecorded = stopwatch.ElapsedMilliseconds;
                }
            }
        }
    }
}
