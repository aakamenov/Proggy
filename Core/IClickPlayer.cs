using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Proggy.Core
{
    public interface IClickPlayer
    {
        bool IsPlaying { get; }
        void Play(IEnumerable<BarInfo> clickData);
        void Stop();
    }
}
