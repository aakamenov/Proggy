using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Proggy.Infrastructure
{
    public class AccurateTimer
    {
        public int Interval { get; set; } = 100;

        private bool isRunning;
        private Action callback;
        private Stopwatch stopwatch;

        public AccurateTimer(Action callback)
        {
            this.callback = callback;
            stopwatch = new Stopwatch();
        }

        public void Start()
        {
            stopwatch.Start();

            isRunning = true;

            Task.Run(() => 
            {
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
            });
        }

        public void Stop()
        {
            isRunning = false;
            stopwatch.Reset();
        }
    }
}
