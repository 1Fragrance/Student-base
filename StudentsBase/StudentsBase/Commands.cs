using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StudentsBase
{
    public class RelayCommand : ICommand
    {
        public RelayCommand(Action<object> _execute, Func<object, bool> _canExecute = null)
        {
            execute = _execute;
            canExecute = _canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        Action<object> execute;
        Func<object, bool> canExecute;
    }
}
