using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Proggy.Core
{
    public interface IClickPlayer
    {
        void Play(int interval, short beatsPerMeasure);
        void Stop();
    }
}
