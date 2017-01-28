namespace WellCompareDir.WPF
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using WellCompareDir.Comparer;
    using WellCompareDir.WPF.Properties;

    /// <summary>
    /// Backing to the main WPF window.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string status;

        private bool outputDirectoryPathIsValid;
        private DirectoryInfo outputDirectory = null;

        // TODO: create an object that collects all left/right data
        // TODO: create a list, where there could also be a center, and a fourth directory comparison
        private FileInfoWithCompareResult left;
        private FileInfoWithCompareResult right;

        private List<FileInfo> leftFileInfos = new List<FileInfo>();
        private List<FileInfo> rightFileInfos = new List<FileInfo>();

        private List<string> fileMask = new List<string>();

        private RelayCommand previousFileCommand;
        private RelayCommand nextFileCommand;
        private RelayCommand previousMatchingFileCommand;
        private RelayCommand nextFileMatchingCommand;
        private RelayCommand useLeftFileCommand;
        private RelayCommand useRightFileCommand;
        private RelayCommand useLeftFileAndAdvanceCommand;
        private RelayCommand useRightFileAndAdvanceCommand;

        private ObservableCollection<FileInfoWithCompareResult> leftFiles = new ObservableCollection<FileInfoWithCompareResult>();
        private ObservableCollection<FileInfoWithCompareResult> rightFiles = new ObservableCollection<FileInfoWithCompareResult>();
        private int selectedFileIndex = 0;
        private bool selectedFileIndexIsInRange = false;
        private string leftImagePath = string.Empty;
        private string rightImagePath = string.Empty;
        private string leftImageDimensions = "-";
        private string rightImageDimensions = "-";
        private string leftFileSize = string.Empty;
        private string rightFileSize = string.Empty;

        public MainWindowViewModel()
        {
            this.AddKnownFiletypes();

            this.Status = string.Empty;
            this.OutputDirectoryPath = string.IsNullOrWhiteSpace(Settings.Default.OutputDirectoryPath) ? this.GetTempDirectory() : Settings.Default.OutputDirectoryPath;
            this.LeftDirectoryPath = string.IsNullOrWhiteSpace(Settings.Default.LeftDirectoryPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : Settings.Default.LeftDirectoryPath;
            this.RightDirectoryPath = string.IsNullOrWhiteSpace(Settings.Default.RightDirectoryPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : Settings.Default.RightDirectoryPath;

            this.InitCommands();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties

        public bool isLeftRecommended { get; set; }

        public bool isRightRecommended { get; set; }

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

        public string LeftDirectoryPath
        {
            get
            {
                return Settings.Default.LeftDirectoryPath;
            }

            set
            {
                Settings.Default.LeftDirectoryPath = value;
                this.OnPropertyChanged("LeftDirectoryPath");
            }
        }

        public string RightDirectoryPath
        {
            get
            {
                return Settings.Default.RightDirectoryPath;
            }

            set
            {
                Settings.Default.RightDirectoryPath = value;
                this.OnPropertyChanged("RightDirectoryPath");
            }
        }

        public ObservableCollection<FileInfoWithCompareResult> LeftFiles
        {
            get
            {
                return this.leftFiles;
            }

            set
            {
                this.leftFiles = value;
                this.OnPropertyChanged("LeftFiles");
            }
        }

        public ObservableCollection<FileInfoWithCompareResult> RightFiles
        {
            get
            {
                return this.rightFiles;
            }

            set
            {
                this.rightFiles = value;
                this.OnPropertyChanged("RightFiles");
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

        public string LeftImagePath
        {
            get
            {
                return this.leftImagePath;
            }

            set
            {
                if (value != this.leftImagePath)
                {
                    this.leftImagePath = value;
                    this.OnPropertyChanged("LeftImagePath");
                }
            }
        }

        public string RightImagePath
        {
            get
            {
                return this.rightImagePath;
            }

            set
            {
                if (value != this.rightImagePath)
                {
                    this.rightImagePath = value;
                    this.OnPropertyChanged("RightImagePath");
                }
            }
        }

        public BitmapImage LeftImageSource
        {
            get
            {
                BitmapImage image = null;
                this.LeftImageDimensions = "-";

                try
                {
                    image = LoadImage(this.LeftImagePath);

                    this.LeftImageDimensions = this.GetImageDimensions(ref image);
                }
                catch
                {
                    // TODO: don't hide errrrrrrors
                }

                return image;
            }
        }

        public BitmapImage RightImageSource
        {
            get
            {
                BitmapImage image = null;
                this.RightImageDimensions = "-";

                try
                {
                    image = LoadImage(this.RightImagePath);

                    this.RightImageDimensions = this.GetImageDimensions(ref image);
                }
                catch
                {
                    // TODO: don't hide errrrrrrors
                }

                return image;
            }
        }

        public string LeftImageDimensions
        {
            get
            {
                return this.leftImageDimensions;
            }

            set
            {
                this.leftImageDimensions = value;
                this.OnPropertyChanged("LeftImageDimensions");
            }
        }

        public string RightImageDimensions
        {
            get
            {
                return this.rightImageDimensions;
            }

            set
            {
                this.rightImageDimensions = value;
                this.OnPropertyChanged("RightImageDimensions");
            }
        }

        public string LeftFileSize
        {
            get
            {
                return this.leftFileSize;
            }

            set
            {
                this.leftFileSize = value;
                this.OnPropertyChanged("LeftFileSize");
            }
        }

        public string RightFileSize
        {
            get
            {
                return this.rightFileSize;
            }

            set
            {
                this.rightFileSize = value;
                this.OnPropertyChanged("RightFileSize");
            }
        }

        public bool IsLeftRecommended
        {
            get
            {
                return this.isLeftRecommended;
            }

            set
            {
                this.isLeftRecommended = value;
                this.OnPropertyChanged("IsLeftRecommended");
            }
        }

        public bool IsRightRecommended
        {
            get
            {
                return this.isRightRecommended;
            }

            set
            {
                this.isRightRecommended = value;
                this.OnPropertyChanged("IsRightRecommended");
            }
        }
        #endregion

        #region Commands

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

        public RelayCommand UseLeftFileCommand
        {
            get
            {
                return this.useLeftFileCommand;
            }

            set
            {
                this.useLeftFileCommand = value;
                this.OnPropertyChanged("UseLeftFileCommand");
            }
        }

        public RelayCommand UseRightFileCommand
        {
            get
            {
                return this.useRightFileCommand;
            }

            set
            {
                this.useRightFileCommand = value;
                this.OnPropertyChanged("UseRightFileCommand");
            }
        }

        public RelayCommand UseLeftFileAndAdvanceCommand
        {
            get
            {
                return this.useLeftFileAndAdvanceCommand;
            }

            set
            {
                this.useLeftFileAndAdvanceCommand = value;
                this.OnPropertyChanged("UseLeftFileAndAdvanceCommand");
            }
        }

        public RelayCommand UseRightFileAndAdvanceCommand
        {
            get
            {
                return this.useRightFileAndAdvanceCommand;
            }

            set
            {
                this.useRightFileAndAdvanceCommand = value;
                this.OnPropertyChanged("UseRightFileAndAdvanceCommand");
            }
        }
        #endregion

        // From
        // https://stackoverflow.com/questions/128618/c-file-size-format-provider
        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

        #region File selection
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
            while (this.CanPreviousFile(parameter))
            {
                this.PreviousFile(parameter);

                if (this.left != null && !this.left.IsUnique
                    && this.right != null && !this.right.IsUnique)
                {
                    break;
                }
            }
        }

        public bool CanNextFile(object parameter)
        {
            return this.SelectedFileIndex < Math.Min(this.LeftFiles.Count - 1, this.RightFiles.Count - 1);
        }

        public void NextFile(object parameter)
        {
            this.SelectedFileIndex = Math.Min(this.SelectedFileIndex + 1, Math.Min(this.LeftFiles.Count - 1, this.RightFiles.Count - 1));
        }

        public void NextMatchingFile(object parameter)
        {
            while (this.CanNextFile(parameter))
            {
                this.NextFile(parameter);

                if (this.left != null && !this.left.IsUnique
                    && this.right != null && !this.right.IsUnique)
                {
                    break;
                }
            }
        }

        public bool CanUseLeftFile(object parameter)
        {
            return this.SelectedFileIndexIsInRange
                       && (!this.LeftFiles[this.SelectedFileIndex].IsEmpty)
                       && this.OutputDirectoryPathIsValid;
        }

        public void UseLeftFile(object parameter)
        {
            FileInfoWithCompareResult left = this.LeftFiles[this.SelectedFileIndex];

            this.UseFile(left);
        }

        public void UseLeftFileAndAdvance(object parameter)
        {
            this.UseLeftFile(parameter);
            this.NextFile(parameter);
        }

        public bool CanUseRightFile(object parameter)
        {
            return this.SelectedFileIndexIsInRange
                        && (!this.RightFiles[this.SelectedFileIndex].IsEmpty)
                        && this.OutputDirectoryPathIsValid;
        }

        public void UseRightFile(object parameter)
        {
            FileInfoWithCompareResult right = this.RightFiles[this.SelectedFileIndex];

            this.UseFile(right);
        }

        public void UseRightFileAndAdvance(object parameter)
        {
            this.UseRightFile(parameter);
            this.NextFile(parameter);
        }

        #endregion

        #region The usual INPC implementation

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

            this.HandlePropertyChange(propertyName);
        }

        #endregion

        #region Misc

        public string GetTempDirectory()
        {
            string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }

        // TODO: convert to extension method
        public string GetFormattedFileSize(ref FileInfo file)
        {
            StringBuilder buffer = new StringBuilder();
            StrFormatByteSize(file.Length, buffer, 100);
            return buffer.ToString();
        }

        #endregion

        // From
        // https://stackoverflow.com/questions/20586/wpf-image-urisource-and-data-binding
        private static BitmapImage LoadImage(string fullPath)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnDemand;
            image.CreateOptions = BitmapCreateOptions.DelayCreation;
            image.UriSource = new Uri(fullPath, UriKind.Absolute);
            image.EndInit();

            // TODO: check possible decoding errors
            ////image.DecodeFailed

            // Used to avoid memory leaks?
            // https://blogs.msdn.microsoft.com/jgoldb/2008/05/05/memory-leaks-in-wpf-based-applications-blog-update/
            if (image.CanFreeze)
            {
                image.Freeze();
            }

            return image;
        }

        // TODO: create extension method
        private static IEnumerable<FileInfo> FilterFileInfosByExtension(DirectoryInfo leftDirectory, IEnumerable<string> extensions)
        {
            List<FileInfo> allFilesInFolder = new List<FileInfo>(leftDirectory.GetFiles());
            IEnumerable<FileInfo> filtered = allFilesInFolder.Where(fi => extensions.Contains(Path.GetExtension(fi.Name).ToLowerInvariant()));
            return filtered;
        }

        #region Property change chaining
        private void HandlePropertyChange(string propertyName)
        {
            if (propertyName == "LeftDirectoryPath")
            {
                this.leftFileInfos.Clear();

                try
                {
                    DirectoryInfo leftDirectory = new DirectoryInfo(this.LeftDirectoryPath);

                    if (leftDirectory.Exists)
                    {
                        this.leftFileInfos.AddRange(FilterFileInfosByExtension(leftDirectory, this.fileMask));
                    }
                }
                catch
                {
                    // TODO: more logging
                }

                this.UpdateFileLists();
                this.SelectedFileIndex = this.LeftFiles.Count > 0 && this.RightFiles.Count > 0 ? 0 : -1;
                CommandManager.InvalidateRequerySuggested();
            }
            else if (propertyName == "RightDirectoryPath")
            {
                this.rightFileInfos.Clear();

                try
                {
                    DirectoryInfo rightDirectory = new DirectoryInfo(this.RightDirectoryPath);

                    if (rightDirectory.Exists)
                    {
                        this.rightFileInfos.AddRange(FilterFileInfosByExtension(rightDirectory, this.fileMask));
                    }
                }
                catch
                {
                    // TODO: more logging
                }

                this.UpdateFileLists();
                this.SelectedFileIndex = this.LeftFiles.Count > 0 && this.RightFiles.Count > 0 ? 0 : -1;
                CommandManager.InvalidateRequerySuggested();
            }
            else if (propertyName == "SelectedFileIndex")
            {
                this.SelectedFileIndexIsInRange =
                    this.SelectedFileIndex != -1
                    && this.SelectedFileIndex < this.LeftFiles.Count
                    && this.SelectedFileIndex < this.RightFiles.Count;

                CommandManager.InvalidateRequerySuggested();
            }
            else if (propertyName == "SelectedFileIndexIsInRange")
            {
                if (this.SelectedFileIndexIsInRange && this.CanUseLeftFile(null))
                {
                    this.left = this.LeftFiles[this.SelectedFileIndex];
                    FileInfo fi = this.left.FileInfo;
                    this.LeftFileSize = this.GetFormattedFileSize(ref fi);
                    this.LeftImagePath = this.left.FileInfo.FullName;
                }
                else
                {
                    this.left = null;
                    this.LeftImagePath = string.Empty;
                    this.LeftFileSize = string.Empty;
                }

                if (this.SelectedFileIndexIsInRange && this.CanUseRightFile(null))
                {
                    this.right = this.RightFiles[this.SelectedFileIndex];
                    FileInfo fi = this.right.FileInfo;
                    this.RightFileSize = this.GetFormattedFileSize(ref fi);
                    this.RightImagePath = this.right.FileInfo.FullName;
                }
                else
                {
                    this.right = null;
                    this.RightImagePath = string.Empty;
                    this.RightFileSize = string.Empty;
                }

                this.UpdateRecommendations();
            }
            else if (propertyName == "OutputDirectoryPath")
            {
                this.outputDirectory = new DirectoryInfo(this.OutputDirectoryPath);

                this.OutputDirectoryPathIsValid = this.outputDirectory.Exists;

                CommandManager.InvalidateRequerySuggested();
            }
            else if (propertyName == "LeftImagePath")
            {
                this.OnPropertyChanged("LeftImageSource");

                this.UpdateRecommendations();
            }
            else if (propertyName == "RightImagePath")
            {
                this.OnPropertyChanged("RightImageSource");

                this.UpdateRecommendations();
            }
        }

        private void UpdateRecommendations()
        {
            this.IsLeftRecommended = false;
            this.IsRightRecommended = false;

            if (this.left == null && this.right == null)
            {
                return;
            }

            if (this.right == null)
            {
                this.IsLeftRecommended = true;

                return;
            }

            if (this.left == null)
            {
                this.IsRightRecommended = true;

                return;
            }

            if (this.LeftImageSource == null && this.RightImageSource == null)
            {
                return;
            }

            if (this.RightImageSource == null)
            {
                this.IsLeftRecommended = true;

                return;
            }

            if (this.LeftImageSource == null)
            {
                this.IsRightRecommended = true;

                return;
            }

            if (this.LeftImageSource.SourceRect.IsEmpty
                && this.RightImageSource.SourceRect.IsEmpty)
            {
                return;
            }

            if (this.RightImageSource.SourceRect.IsEmpty)
            {
                this.IsLeftRecommended = true;

                return;
            }

            if (this.LeftImageSource.SourceRect.IsEmpty)
            {
                this.IsRightRecommended = true;

                return;
            }

            if (this.LeftImageSource.Width > this.RightImageSource.Width
                && this.LeftImageSource.Height > this.RightImageSource.Height)
            {
                this.IsLeftRecommended = true;

                return;
            }

            if (this.LeftImageSource.Width < this.RightImageSource.Width
                && this.LeftImageSource.Height < this.RightImageSource.Height)
            {
                this.IsRightRecommended = true;

                return;
            }

            if (this.left.FileInfo.Length > this.right.FileInfo.Length)
            {
                this.IsLeftRecommended = true;

                return;
            }

            if (this.left.FileInfo.Length < this.right.FileInfo.Length)
            {
                this.IsRightRecommended = true;

                return;
            }
        }
        #endregion

        private void AddKnownFiletypes()
        {
            // TODO: let user add more in through App.config?
            this.fileMask.Add(".bmp");
            this.fileMask.Add(".jpg");
            this.fileMask.Add(".jpeg");
            this.fileMask.Add(".jpe");
            this.fileMask.Add(".png");
            this.fileMask.Add(".tif");
            this.fileMask.Add(".tiff");
            this.fileMask.Add(".hdp");
            this.fileMask.Add(".wdp");
            this.fileMask.Add(".jxr");
            this.fileMask.Add(".gif");
            this.fileMask.Add(".ico");
        }

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

        private void UpdateFileLists()
        {
            NameWithoutExtensionComparer comparer = new NameWithoutExtensionComparer();

            DirectoryComparer directoryComparer = new DirectoryComparer(this.leftFileInfos, this.rightFileInfos, comparer);

            IEnumerable<FileInfo> leftUnique = directoryComparer.GetUniqueLeft();
            IEnumerable<FileInfo> rightUnique = directoryComparer.GetUniqueRight();

            this.LeftFiles.Clear();
            this.RightFiles.Clear();

            var left = this.leftFileInfos.GetEnumerator();
            var right = this.rightFileInfos.GetEnumerator();

            if (this.leftFileInfos.Count > 0 && this.rightFileInfos.Count > 0)
            {
                right.MoveNext();

                while (left.MoveNext() && right.Current != null)
                {
                    int cmpOuter = comparer.Compare(left.Current, right.Current);

                    if (cmpOuter < 0)
                    {
                        this.LeftFiles.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                        this.RightFiles.Add(new FileInfoWithCompareResult());
                    }
                    else if (cmpOuter == 0)
                    {
                        this.LeftFiles.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                        this.RightFiles.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));

                        right.MoveNext();
                    }
                    else
                    {
                        this.LeftFiles.Add(new FileInfoWithCompareResult());
                        this.RightFiles.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));

                        bool canRightMoveNext = right.MoveNext();

                        while (canRightMoveNext)
                        {
                            int cmpInner = comparer.Compare(left.Current, right.Current);

                            if (cmpInner > 0)
                            {
                                this.LeftFiles.Add(new FileInfoWithCompareResult());
                                this.RightFiles.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));
                            }
                            else if (cmpInner == 0)
                            {
                                this.LeftFiles.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                                this.RightFiles.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));

                                left.MoveNext();
                            }
                            else
                            {
                                this.LeftFiles.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                                this.RightFiles.Add(new FileInfoWithCompareResult());

                                break;
                            }

                            canRightMoveNext = right.MoveNext();
                        }

                        if (!canRightMoveNext)
                        {
                            this.LeftFiles.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                            this.RightFiles.Add(new FileInfoWithCompareResult());
                        }
                    }
                }
            }

            if (left.Current != null)
            {
                do
                {
                    this.LeftFiles.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                    this.RightFiles.Add(new FileInfoWithCompareResult());
                }
                while (left.MoveNext());
            }

            if (right.Current != null)
            {
                do
                {
                    this.LeftFiles.Add(new FileInfoWithCompareResult());
                    this.RightFiles.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));
                }
                while (right.MoveNext());
            }
        }
        #endregion

        private void InitCommands()
        {
            this.PreviousFileCommand = new RelayCommand(this.PreviousFile, this.CanPreviousFile);
            this.NextFileCommand = new RelayCommand(this.NextFile, this.CanNextFile);
            this.PreviousMatchingFileCommand = new RelayCommand(this.PreviousMatchingFile, this.CanPreviousFile);
            this.NextMatchingFileCommand = new RelayCommand(this.NextMatchingFile, this.CanNextFile);
            this.UseLeftFileCommand = new RelayCommand(this.UseLeftFile, this.CanUseLeftFile);
            this.UseRightFileCommand = new RelayCommand(this.UseRightFile, this.CanUseRightFile);
            this.UseLeftFileAndAdvanceCommand = new RelayCommand(this.UseLeftFileAndAdvance, this.CanUseLeftFile);
            this.UseRightFileAndAdvanceCommand = new RelayCommand(this.UseRightFileAndAdvance, this.CanUseRightFile);
        }

        // TODO: convert to extension method
        private string GetImageDimensions(ref BitmapImage image)
        {
            return string.Format("{0}x{1}px ({2}x{3} dpi)", image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY);
        }
    }
}
