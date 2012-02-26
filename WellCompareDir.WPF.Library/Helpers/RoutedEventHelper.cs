namespace WellCompareDir.WPF.Library.Helpers
{
    using System.Windows.Input;

    public class RoutedEventHelper
    {
        public static void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
