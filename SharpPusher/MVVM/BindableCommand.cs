// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Windows.Input;

namespace SharpPusher.MVVM
{
    public class BindableCommand : ICommand
    {
        public BindableCommand(Action executeMethod) : this(executeMethod, null)
        {
        }

        public BindableCommand(Action executeMethod, Func<bool>? canExecuteMethod)
        {
            ExecuteMethod = executeMethod;
            CanExecuteMethod = canExecuteMethod;
        }


        private readonly Action ExecuteMethod;
        private readonly Func<bool>? CanExecuteMethod;

        public event EventHandler? CanExecuteChanged;


        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public bool CanExecute(object? parameter) => CanExecuteMethod is null || CanExecuteMethod();

        public void Execute(object? parameter) => ExecuteMethod?.Invoke();
    }
}
