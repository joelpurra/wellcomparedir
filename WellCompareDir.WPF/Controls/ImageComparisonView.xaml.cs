using System.Windows.Controls;
using System.Windows.Input;
using WellCompareDir.WPF.Library.Helpers;

namespace WellCompareDir.WPF
{
    /// <summary>
    /// Interaction logic for ImageComparison.xaml
    /// </summary>
    public partial class ImageComparisonView : UserControl
    {
        public static RoutedUICommand BrowseForDirectoryCommand = new RoutedUICommand();

        public ImageComparisonView()
        {
            InitializeComponent();

            this.InitCommands();
        }

        private void files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.files.SelectedItem != null)
            {
                this.files.ScrollIntoView(this.files.SelectedItem);
            }

            if (this.useButton != null)
            {
                this.useButton.Focus();
            }
        }

        private void InitCommands()
        {
            this.CommandBindings.Add(new CommandBinding(BrowseForDirectoryCommand, this.BrowseForDirectory, RoutedEventHelper.CanAlwaysExecute));
        }

        public void BrowseForDirectory(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: be less aware of model?
            ImageComparisonViewModel icvm = this.DataContext as ImageComparisonViewModel;

            icvm.DirectoryPath = FolderHelper.BrowseForFolder("Select left directory path", icvm.DirectoryPath) ?? icvm.DirectoryPath;
        }
    }
}
