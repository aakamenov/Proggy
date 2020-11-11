using System;
using System.Collections.Generic;
using System.Text;

namespace Proggy.Infrastructure
{
    public interface IDialog
    {
        Action Close { get; set; }
        bool WasClosedFromView { get; set; }
    }
}
