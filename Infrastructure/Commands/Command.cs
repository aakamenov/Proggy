using System;
using System.Windows.Input;

namespace Proggy.Infrastructure.Commands
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Predicate<object> CanExecuteFunc { get; set; }

        private readonly Action execute;

        public Command(Action execute) : this(execute, null) { }

        public Command(Action execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            CanExecuteFunc = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null ? true : CanExecuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            execute();
        }
    }

    public class Command<TArg> : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Predicate<object> CanExecuteFunc { get; set; }

        private readonly Action<TArg> execute;

        public Command(Action<TArg> execute) : this(execute, null) { }

        public Command(Action<TArg> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            CanExecuteFunc = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null ? true : CanExecuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            execute((TArg)parameter);
        }
    }
}
