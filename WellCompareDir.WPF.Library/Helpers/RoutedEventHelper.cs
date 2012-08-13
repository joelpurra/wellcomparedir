namespace WellCompareDir.WPF.Library.Helpers
{
    using System.Diagnostics;
    using System.Windows.Input;

    public class RoutedEventHelper
    {
        [DebuggerStepThrough]
        public static void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
