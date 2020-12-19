using ReactiveUI;

namespace Proggy.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        public virtual void OnClosing() { }
    }
}
