namespace WellCompareDir.WPF
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;
    using WellCompareDir.Comparer;
    using WellCompareDir.WPF.Library;
    using WellCompareDir.WPF.Library.Extensions;
    using WellCompareDir.WPF.Properties;
    using WellCompareDir.WPF.ViewModels;

    /// <summary>
    /// Backing to the main WPF window.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        // TODO: make configuable from the view
        private const int MaxComparisons = 4;

        private string status;

        private bool outputDirectoryPathIsValid;
        private DirectoryInfo outputDirectory = null;

        private List<string> DirectoryPaths;
        private ObservableCollection<ImageComparisonViewModel> comparisions;

        private RelayCommand addComparisonCommand;
        private RelayCommand previousFileCommand;
        private RelayCommand nextFileCommand;
        private RelayCommand previousMatchingFileCommand;
        private RelayCommand nextFileMatchingCommand;

        private int selectedFileIndex = 0;
        private bool selectedFileIndexIsInRange = false;

        public MainWindowViewModel()
        {
            this.comparisions = new ObservableCollection<ImageComparisonViewModel>();

            this.Status = string.Empty;
            this.ReadSettings();
            this.InitCommands();
        }

        private void ReadSettings()
        {
            this.OutputDirectoryPath = string.IsNullOrWhiteSpace(Settings.Default.OutputDirectoryPath) ? this.GetTempDirectory() : Settings.Default.OutputDirectoryPath;

            // TODO: make sure the settings are written back from the comparison controls
            this.DirectoryPaths = new List<string>(StringCollectionAsIEnumerableString(Settings.Default.DirectoryPaths));
        }

        public void AddComparison(object parameter = null)
        {
            this.AddComparison(this.comparisions.Count);
        }

        public void AddComparison(int i)
        {
            this.comparisions.Add(new ImageComparisonViewModel());

            this.GetDirectoryPathOrDefault(i);
        }

        private void GetDirectoryPathOrDefault(int i)
        {
            this.GetComparison(i).DirectoryPath =
                this.DirectoryPaths.Count < i + 1 || string.IsNullOrWhiteSpace(this.DirectoryPaths[i])
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                : this.DirectoryPaths[i];
        }

        private IEnumerable<ImageComparisonViewModel> GetComparisons()
        {
            return this.comparisions;
        }

        private ImageComparisonViewModel GetComparison(int i)
        {
            return this.comparisions[i];
        }

        private static IEnumerable<string> StringCollectionAsIEnumerableString(StringCollection collection)
        {
            List<string> strs = new List<string>();

            if (collection != null)
            {
                foreach (string str in collection)
                {
                    strs.Add(str);
                }
            }

            return strs;
        }

        #region Properties

        public ObservableCollection<ImageComparisonViewModel> Comparisons
        {
            get
            {
                return this.comparisions;
            }

            set
            {
                this.comparisions = value;
                this.OnPropertyChanged("Comparisons");
            }
        }

        public string Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.status = value;
                this.OnPropertyChanged("Status");
            }
        }

        public string OutputDirectoryPath
        {
            get
            {
                return Settings.Default.OutputDirectoryPath;
            }

            set
            {
                Settings.Default.OutputDirectoryPath = value;
                this.OnPropertyChanged("OutputDirectoryPath");
            }
        }

        public bool OutputDirectoryPathIsValid
        {
            get
            {
                return this.outputDirectoryPathIsValid;
            }

            set
            {
                this.outputDirectoryPathIsValid = value;
                this.OnPropertyChanged("OutputDirectoryPathIsValid");
            }
        }

        public int SelectedFileIndex
        {
            get
            {
                return this.selectedFileIndex;
            }

            set
            {
                this.selectedFileIndex = value;
                this.OnPropertyChanged("SelectedFileIndex");
            }
        }

        public bool SelectedFileIndexIsInRange
        {
            get
            {
                return this.selectedFileIndexIsInRange;
            }

            set
            {
                this.selectedFileIndexIsInRange = value;
                this.OnPropertyChanged("SelectedFileIndexIsInRange");
            }
        }
        #endregion

        #region Commands

        public RelayCommand AddComparisonCommand
        {
            get
            {
                return this.addComparisonCommand;
            }

            set
            {
                this.addComparisonCommand = value;
                this.OnPropertyChanged("AddComparisonCommand");
            }
        }

        public RelayCommand PreviousFileCommand
        {
            get
            {
                return this.previousFileCommand;
            }

            set
            {
                this.previousFileCommand = value;
                this.OnPropertyChanged("PreviousFileCommand");
            }
        }

        public RelayCommand NextFileCommand
        {
            get
            {
                return this.nextFileCommand;
            }

            set
            {
                this.nextFileCommand = value;
                this.OnPropertyChanged("NextFileCommand");
            }
        }

        public RelayCommand PreviousMatchingFileCommand
        {
            get
            {
                return this.previousMatchingFileCommand;
            }

            set
            {
                this.previousMatchingFileCommand = value;
                this.OnPropertyChanged("PreviousMatchingFileCommand");
            }
        }

        public RelayCommand NextMatchingFileCommand
        {
            get
            {
                return this.nextFileMatchingCommand;
            }

            set
            {
                this.nextFileMatchingCommand = value;
                this.OnPropertyChanged("NextMatchingFileCommand");
            }
        }

        #endregion

        public bool CanAddComparison(object parameter)
        {
            return (this.comparisions.Count < MaxComparisons);
        }

        #region File selection

        private int CountExisting(int index)
        {
            Contract.Requires(index >= 0);
            Contract.Ensures(Contract.Result<int>() >= 0);

            int existing = 0;

            // TODO: convert to link with predicates
            foreach (ImageComparisonViewModel ic in this.GetComparisons())
            {
                var fiwcr = ic.GetFileInfoWithCompareResult(index);

                if (fiwcr != null)
                {
                    existing++;
                }
            }

            return existing;
        }

        private int MinLength()
        {
            if (this.comparisions.Count == 0)
            {
                return 0;
            }

            return this.GetComparisons().Min(ic => ic.Files.Count);
        }

        private int MaxLength()
        {
            if (this.comparisions.Count == 0)
            {
                return 0;
            }

            return this.GetComparisons().Max(ic => ic.Files.Count);
        }

        public bool CanPreviousFile(object parameter)
        {
            return this.SelectedFileIndex > 0;
        }

        public void PreviousFile(object parameter)
        {
            this.SelectedFileIndex = Math.Max(this.SelectedFileIndex - 1, 0);
        }

        public void PreviousMatchingFile(object parameter)
        {
            // TODO: pause rendering in loop?
            while (this.CanPreviousFile(parameter))
            {
                this.PreviousFile(parameter);

                int countExisting = this.CountExisting(this.SelectedFileIndex);

                if (countExisting >= 2)
                {
                    break;
                }
            }
        }

        public bool CanNextFile(object parameter)
        {
            return this.SelectedFileIndex < this.MaxLength() - 1;
        }

        public void NextFile(object parameter)
        {
            this.SelectedFileIndex = Math.Min(this.SelectedFileIndex + 1, this.MaxLength() - 1);
        }

        public void NextMatchingFile(object parameter)
        {
            // TODO: pause rendering in loop?
            while (this.CanNextFile(parameter))
            {
                this.NextFile(parameter);

                int countExisting = this.CountExisting(this.SelectedFileIndex);

                if (countExisting >= 2)
                {
                    break;
                }
            }
        }

        #endregion

        #region Misc

        public string GetTempDirectory()
        {
            string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }

        #endregion

        #region Property change chaining

        protected override void HandlePropertyChange(string propertyName)
        {
            if (propertyName == "SelectedFileIndex")
            {
                this.SelectedFileIndexIsInRange =
                    this.SelectedFileIndex >= 0
                    && this.GetComparisons().Any(cmp => this.SelectedFileIndex < cmp.Files.Count);

                CommandManager.InvalidateRequerySuggested();
            }
            else if (propertyName == "OutputDirectoryPath")
            {
                this.outputDirectory = new DirectoryInfo(this.OutputDirectoryPath);

                this.OutputDirectoryPathIsValid = this.outputDirectory.Exists;

                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void UpdateRecommendations()
        {
            // Reset all
            this.GetComparisons().All(ic => ic.IsRecommended = false);

            IEnumerable<ImageComparisonViewModel> filtered = new List<ImageComparisonViewModel>(this.GetComparisons());

            int index = this.SelectedFileIndex;

            ImageComparisonViewModel noNull = IEnumerableExtensions.FilterOnlyOrDefault(ref filtered, ic => ic.GetFileInfoWithCompareResult(index) != null);

            ImageComparisonViewModel noEmptyImage = IEnumerableExtensions.FilterOnlyOrDefault(ref filtered, ic => ic.ImageSource.SourceRect.IsEmpty);

            long maxArea = filtered.Max(ic => ic.ImageSource.Area());
            ImageComparisonViewModel biggestImageArea = IEnumerableExtensions.FilterOnlyOrDefault(ref filtered, ic => ic.ImageSource.Area() == maxArea);

            long maxFileSize = filtered.Max(ic => ic.GetFileInfoWithCompareResult(this.SelectedFileIndex).FileInfo.Length);
            ImageComparisonViewModel biggestFileSize = IEnumerableExtensions.FilterOnlyOrDefault(ref filtered, ic => ic.GetFileInfoWithCompareResult(this.SelectedFileIndex).FileInfo.Length == maxFileSize);
        }

        #endregion

        private bool UseFile(FileInfoWithCompareResult file)
        {
            try
            {
                if (!file.IsEmpty
                    && file.FileInfo != null
                    && file.FileInfo.Exists)
                {
                    File.Copy(file.FileInfo.FullName, Path.Combine(this.outputDirectory.FullName, file.FileInfo.Name));

                    return true;
                }
            }
            catch
            {
                // TODO: error reporting
            }

            return false;
        }

        #region UpdateFileLists

        public void UpdateFileLists()
        {
            NameWithoutExtensionComparer comparer = new NameWithoutExtensionComparer();

            foreach (ImageComparisonViewModel outer in this.GetComparisons())
            {
                foreach (ImageComparisonViewModel inner in this.GetComparisons())
                {
                    if (outer == inner)
                    {
                        continue;
                    }

                    DirectoryComparer directoryComparer = new DirectoryComparer(outer.FileInfos, inner.FileInfos, comparer);

                    IEnumerable<FileInfo> leftUnique = directoryComparer.GetUniqueLeft();
                    IEnumerable<FileInfo> rightUnique = directoryComparer.GetUniqueRight();

                    outer.Files.Clear();
                    inner.Files.Clear();

                    var left = inner.FileInfos.GetEnumerator();
                    var right = outer.FileInfos.GetEnumerator();

                    if (inner.FileInfos.Count > 0 && outer.FileInfos.Count > 0)
                    {
                        right.MoveNext();

                        while (left.MoveNext() && right.Current != null)
                        {
                            int cmpOuter = comparer.Compare(left.Current, right.Current);

                            if (cmpOuter < 0)
                            {
                                inner.Files.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                                outer.Files.Add(new FileInfoWithCompareResult());
                            }
                            else if (cmpOuter == 0)
                            {
                                inner.Files.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                                outer.Files.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));

                                right.MoveNext();
                            }
                            else
                            {
                                inner.Files.Add(new FileInfoWithCompareResult());
                                outer.Files.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));

                                bool canRightMoveNext = right.MoveNext();

                                while (canRightMoveNext)
                                {
                                    int cmpInner = comparer.Compare(left.Current, right.Current);

                                    if (cmpInner > 0)
                                    {
                                        inner.Files.Add(new FileInfoWithCompareResult());
                                        outer.Files.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));
                                    }
                                    else if (cmpInner == 0)
                                    {
                                        inner.Files.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                                        outer.Files.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));

                                        left.MoveNext();
                                    }
                                    else
                                    {
                                        inner.Files.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                                        outer.Files.Add(new FileInfoWithCompareResult());

                                        break;
                                    }

                                    canRightMoveNext = right.MoveNext();
                                }

                                if (!canRightMoveNext)
                                {
                                    inner.Files.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                                    outer.Files.Add(new FileInfoWithCompareResult());
                                }
                            }
                        }
                    }

                    if (left.Current != null)
                    {
                        do
                        {
                            inner.Files.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                            outer.Files.Add(new FileInfoWithCompareResult());
                        }
                        while (left.MoveNext());
                    }

                    if (right.Current != null)
                    {
                        do
                        {
                            inner.Files.Add(new FileInfoWithCompareResult());
                            outer.Files.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));
                        }
                        while (right.MoveNext());
                    }
                }
            }
        }
        #endregion

        private void InitCommands()
        {
            this.AddComparisonCommand = new RelayCommand(this.AddComparison, this.CanAddComparison);
            this.PreviousFileCommand = new RelayCommand(this.PreviousFile, this.CanPreviousFile);
            this.NextFileCommand = new RelayCommand(this.NextFile, this.CanNextFile);
            this.PreviousMatchingFileCommand = new RelayCommand(this.PreviousMatchingFile, this.CanPreviousFile);
            this.NextMatchingFileCommand = new RelayCommand(this.NextMatchingFile, this.CanNextFile);
        }
    }
}
