using System;
using System.Windows.Input;

namespace DatabaseConnectionDialog
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

}
