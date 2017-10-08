using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TowersOfHanoi {
	public class RelayCommand<T> : ICommand {
		public bool CanExecute(object parameter) {
			return _canExecute == null ? true : _canExecute((T)parameter);
		}
		public event EventHandler CanExecuteChanged {
			add {
				if(_canExecute != null)
					CommandManager.RequerySuggested += value;
			}
			remove {
				if(_canExecute != null)
					CommandManager.RequerySuggested -= value;
			}
		}
		public void Execute(object parameter) {
			_execute((T)parameter);
		}
		Action<T> _execute;
		Func<T, bool> _canExecute;
		public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null) {
			_execute = execute;
			_canExecute = canExecute;
		}
	}

	public class RelayCommand : RelayCommand<object> {
		public RelayCommand(Action execute, Func<bool> canExecute = null)
			: base(_ => execute(), canExecute == null ? null :
		(Func<object, bool>)(_ => canExecute())) {
		}
	}

}
