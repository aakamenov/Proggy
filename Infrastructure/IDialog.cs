using System;

namespace Proggy.Infrastructure
{
    public interface IDialog
    {
        Action Close { get; set; }
        bool IsConfirm { get; set; }
    }
}
