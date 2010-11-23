namespace WellCompareDir.WPF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Windows.Forms;

    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();

            InitCommands();
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

        public static RoutedUICommand OpenAboutWindowCommand = new RoutedUICommand();
        public static RoutedUICommand BrowseForOutputDirectoryCommand = new RoutedUICommand();
        public static RoutedUICommand BrowseForLeftDirectoryCommand = new RoutedUICommand();
        public static RoutedUICommand BrowseForRightDirectoryCommand = new RoutedUICommand();

        private void InitCommands()
        {
            this.CommandBindings.Add(new CommandBinding(OpenAboutWindowCommand, OpenAboutWindow, CanAlwaysExecute));

            this.CommandBindings.Add(new CommandBinding(BrowseForOutputDirectoryCommand, BrowseForOutputDirectory, CanAlwaysExecute));
            this.CommandBindings.Add(new CommandBinding(BrowseForLeftDirectoryCommand, BrowseForLeftDirectory, CanAlwaysExecute));
            this.CommandBindings.Add(new CommandBinding(BrowseForRightDirectoryCommand, BrowseForRightDirectory, CanAlwaysExecute));
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
            this.outputDirectoryPath.Content = this.BrowseForFolder("Select output directory path", this.outputDirectoryPath.Content as string) ?? this.outputDirectoryPath.Content as string;
        }

        public void BrowseForLeftDirectory(object sender, ExecutedRoutedEventArgs e)
        {
            this.leftDirectoryPath.Content = this.BrowseForFolder("Select left directory path", this.leftDirectoryPath.Content as string) ?? this.leftDirectoryPath.Content as string;
        }

        public void BrowseForRightDirectory(object sender, ExecutedRoutedEventArgs e)
        {
            this.rightDirectoryPath.Content = this.BrowseForFolder("Select right directory path", this.rightDirectoryPath.Content as string) ?? this.rightDirectoryPath.Content as string;
        }
    }
}
