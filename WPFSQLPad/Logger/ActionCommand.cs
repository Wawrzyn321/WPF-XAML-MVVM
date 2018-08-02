using System;
using System.Windows.Input;

namespace Logger
{
    /// <summary>
    /// ICommand implementation
    /// </summary>
    public class ActionCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public void Execute(object parameter)
        {
            execute();
        }

        public event EventHandler CanExecuteChanged;

        #endregion

        public ActionCommand(Action execute)
        {
            this.execute = execute;
            canExecute = () => true;
        }

        public ActionCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

    }


    /// <summary>
    /// Generic ICommand implementation
    /// </summary>
    public class ActionCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<bool> canExecute;

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public void Execute(object parameter)
        {
            try
            {
                execute((T)parameter);
            }
            catch (InvalidCastException)
            {
                // ignored
            }
        }

        public event EventHandler CanExecuteChanged;

        #endregion

        public ActionCommand(Action<T> execute)
        {
            this.execute = execute;
            canExecute = () => true;
        }

        public ActionCommand(Action<T> execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

    }

}
