namespace WellCompareDir.WPF
{
    using System;
    using System.Windows.Input;

    // From
    // http://stackoverflow.com/questions/3826651/how-to-grab-keyboard-from-child-controls-in-wpf
    public class RelayCommand : ICommand
    {
        private Action<object> _exec;
        private Func<object, bool> _canExec;

        public RelayCommand(Action<object> exec, Func<object, bool> canExec)
        {
            _exec = exec;
            _canExec = canExec;
        }

        public void Execute(object parameter)
        {
            _exec(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExec(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
