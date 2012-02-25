namespace WellCompareDir.WPF
{
    using System.Windows;
    using System.Windows.Input;
    using WellCompareDir.WPF.Library.Helpers;

    public partial class MainWindowView : Window
    {
        public static RoutedUICommand OpenAboutWindowCommand = new RoutedUICommand();
        public static RoutedUICommand BrowseForOutputDirectoryCommand = new RoutedUICommand();

        public MainWindowView()
        {
            InitializeComponent();

            this.InitCommands();
        }

        private MainWindowViewModel Model
        {
            get
            {
                return this.DataContext as MainWindowViewModel;
            }
        }

        public void BrowseForOutputDirectory(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: be less aware of model
            this.Model.OutputDirectoryPath =
                FolderHelper.BrowseForFolder("Select output directory path", this.Model.OutputDirectoryPath)
                ?? this.Model.OutputDirectoryPath;
        }

        private void InitCommands()
        {
            this.CommandBindings.Add(new CommandBinding(OpenAboutWindowCommand, this.OpenAboutWindow, RoutedEventHelper.CanAlwaysExecute));

            this.CommandBindings.Add(new CommandBinding(BrowseForOutputDirectoryCommand, this.BrowseForOutputDirectory, RoutedEventHelper.CanAlwaysExecute));
        }

        private void OpenAboutWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Window aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }
    }
}
