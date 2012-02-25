namespace WellCompareDir.WPF.Library
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    ///     Relaying key bindings.
    ///     From https://stackoverflow.com/questions/1023960/keybinding-a-relaycommand.
    /// </summary>
    public class RelayKeyBinding : KeyBinding
    {
        public static readonly DependencyProperty CommandBindingProperty =
            DependencyProperty.Register(
                "CommandBinding",
                typeof(ICommand),
                typeof(RelayKeyBinding),
            new FrameworkPropertyMetadata(OnCommandBindingChanged));

        public ICommand CommandBinding
        {
            get { return (ICommand)GetValue(CommandBindingProperty); }
            set { SetValue(CommandBindingProperty, value); }
        }

        private static void OnCommandBindingChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var keyBinding = (RelayKeyBinding)d;
            keyBinding.Command = (ICommand)e.NewValue;
        }
    }
}
