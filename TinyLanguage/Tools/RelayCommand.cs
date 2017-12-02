using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace  Freel.Tools
{
    public class RelayCommand : ICommand
    {

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException();
            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }


        bool ICommand.CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
