namespace WellCompareDir.WPF
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;
    using WellCompareDir.WPF.Library.Helpers;

    public partial class MainWindowView : Window, INotifyPropertyChanged
    {
        #region The usual INPC implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

            this.HandlePropertyChange(propertyName);
        }

        private void HandlePropertyChange(string propertyName)
        {
            // Nothing needs to be done here right now
        }

        #endregion

        public static RoutedUICommand OpenAboutWindowCommand = new RoutedUICommand();

        public static RoutedUICommand BrowseForOutputDirectoryCommand = new RoutedUICommand();

        public MainWindowView()
        {
            InitializeComponent();

            this.InitCommands();

            this.ListenToModel();
        }

        private void ListenToModel()
        {
            this.Model.Comparisons.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Comparisons_CollectionChanged);
        }

        void Comparisons_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("ComparisonColumns");
            this.OnPropertyChanged("ComparisonRows");
        }

        public int ComparisonColumns
        {
            get
            {
                return this.Model.Comparisons.Count < 2 ? 1 : 2;
            }
        }

        public int ComparisonRows
        {
            get
            {
                return this.Model.Comparisons.Count < 3 ? 1 : 2;
            }
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

        private void ItemsControl_DirectorySelected(object sender, RoutedEventArgs e)
        {
            this.Model.UpdateFileLists();
        }
    }
}
