using System.Windows.Input;

namespace RemoteMonitor.ViewModel
{
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        /// <summary>
        /// Event that is triggered when the CanExecute value changes
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute; // The action to execute
            _canExecute = canExecute; // The function to check if the action can be executed
        }

        /// <summary>
        /// Checks if the action can be executed
        /// </summary>
        /// <param name="parameter">The parameter to pass to the action</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Executes the action
        /// </summary>
        /// <param name="parameter">The parameter to pass to the action</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
