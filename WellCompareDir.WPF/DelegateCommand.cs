﻿namespace WellCompareDir.WPF
{
    using System;
    using System.Windows.Input;

    // From
    // https://relentlessdevelopment.wordpress.com/2010/03/30/simplified-mvvm-commanding-with-delegatecommand/
    public class DelegateCommand : ICommand
    {
        private Action _executeMethod;
        public DelegateCommand(Action executeMethod)
        {
            _executeMethod = executeMethod;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            _executeMethod.Invoke();
        }
    }
}
