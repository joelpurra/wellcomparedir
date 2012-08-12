namespace WellCompareDir.WPF
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Input;

    public partial class MainWindowView : Window
    {
        public static RoutedUICommand OpenAboutWindowCommand = new RoutedUICommand();
        public static RoutedUICommand BrowseForOutputDirectoryCommand = new RoutedUICommand();
        public static RoutedUICommand BrowseForLeftDirectoryCommand = new RoutedUICommand();
        public static RoutedUICommand BrowseForRightDirectoryCommand = new RoutedUICommand();

        public MainWindowView()
        {
            InitializeComponent();

            this.InitCommands();
        }

        public string BrowseForFolder(string description, string start)
        {
            string selected = null;

            FolderBrowserDialog dlg = new FolderBrowserDialog();

            dlg.Description = description;

            dlg.SelectedPath = start;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selected = dlg.SelectedPath;
            }

            return selected;
        }

        public void BrowseForOutputDirectory(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: be less aware of model
            MainWindowViewModel mwvm = this.DataContext as MainWindowViewModel;
            mwvm.OutputDirectoryPath = this.BrowseForFolder("Select output directory path", mwvm.OutputDirectoryPath) ?? mwvm.OutputDirectoryPath;
        }

        public void BrowseForLeftDirectory(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: be less aware of model
            MainWindowViewModel mwvm = this.DataContext as MainWindowViewModel;
            mwvm.LeftDirectoryPath = this.BrowseForFolder("Select left directory path", mwvm.LeftDirectoryPath) ?? mwvm.LeftDirectoryPath;
        }

        public void BrowseForRightDirectory(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: be less aware of model
            MainWindowViewModel mwvm = this.DataContext as MainWindowViewModel;
            mwvm.RightDirectoryPath = this.BrowseForFolder("Select right directory path", mwvm.RightDirectoryPath) ?? mwvm.RightDirectoryPath;
        }

        private void InitCommands()
        {
            this.CommandBindings.Add(new CommandBinding(OpenAboutWindowCommand, this.OpenAboutWindow, this.CanAlwaysExecute));

            this.CommandBindings.Add(new CommandBinding(BrowseForOutputDirectoryCommand, this.BrowseForOutputDirectory, this.CanAlwaysExecute));
            this.CommandBindings.Add(new CommandBinding(BrowseForLeftDirectoryCommand, this.BrowseForLeftDirectory, this.CanAlwaysExecute));
            this.CommandBindings.Add(new CommandBinding(BrowseForRightDirectoryCommand, this.BrowseForRightDirectory, this.CanAlwaysExecute));
        }

        private void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenAboutWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Window aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        private void leftFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.leftFiles.SelectedItem != null)
            {
                this.leftFiles.ScrollIntoView(this.leftFiles.SelectedItem);
            }

            if (this.useLeftButton != null)
            {
                this.useLeftButton.Focus();
            }
        }

        private void rightFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.rightFiles.SelectedItem != null)
            {
                this.rightFiles.ScrollIntoView(this.rightFiles.SelectedItem);
            }

            if (this.useRightButton != null)
            {
                this.useRightButton.Focus();
            }
        }
    }
}
