using System;
using System.Windows.Input;

namespace MVVMLib
{
    public class BindableCommand : ICommand
    {
        public BindableCommand(Action actionToExecute)
        {
            methodToExecute = actionToExecute;
        }
        public BindableCommand(Action actionToExecute, Func<bool> canExecute)
        {
            methodToExecute = actionToExecute;
            canExecuteMethod = canExecute;
        }


        private readonly Action methodToExecute;
        private readonly Func<bool> canExecuteMethod;


        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }


        #region ICommand

        public bool CanExecute(object parameter)
        {
            if (canExecuteMethod != null)
            {
                return canExecuteMethod();
            }
            return methodToExecute != null;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (methodToExecute != null)
            {
                methodToExecute();
            }
        }

        #endregion
    }




    public class BindableCommand<T> : ICommand
    {
        public BindableCommand(Action<T> parameterizedAction)
        {
            methodToExecute = parameterizedAction;
        }
        public BindableCommand(Action<T> parameterizedAction, Func<T, bool> canExecute)
        {
            methodToExecute = parameterizedAction;
            canExecuteMethod = canExecute;
        }


        private readonly Action<T> methodToExecute;
        private readonly Func<T, bool> canExecuteMethod;


        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }


        #region ICommand

        public bool CanExecute(object parameter)
        {
            if (canExecuteMethod != null)
            {
                return canExecuteMethod((T)parameter);
            }
            return methodToExecute != null;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (methodToExecute != null)
            {
                methodToExecute((T)parameter);
            }
        }

        #endregion
    }
}
