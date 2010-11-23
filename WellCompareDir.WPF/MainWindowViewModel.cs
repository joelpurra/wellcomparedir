using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WellCompareDir.Comparer;
using System.Runtime.InteropServices;
using System.Text;
using WellCompareDir.WPF.Properties;

namespace WellCompareDir.WPF
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        DirectoryInfo outputDirectory = null;

        // TODO: create an object that collects all left/right data
        // TODO: create a list, where there could also be a center, and a fourth directory comparison

        FileInfoWithCompareResult left;
        FileInfoWithCompareResult right;

        List<FileInfo> leftFileInfos = new List<FileInfo>();
        List<FileInfo> rightFileInfos = new List<FileInfo>();

        List<string> FileMask = new List<string>();

        public MainWindowViewModel()
        {
            this.FileMask.Add(".bmp");
            this.FileMask.Add(".jpg");
            this.FileMask.Add(".jpeg");
            this.FileMask.Add(".jpe");
            this.FileMask.Add(".png");
            this.FileMask.Add(".tif");
            this.FileMask.Add(".tiff");
            this.FileMask.Add(".hdp");
            this.FileMask.Add(".wdp");
            this.FileMask.Add(".jxr");
            this.FileMask.Add(".gif");
            this.FileMask.Add(".ico");

            this.Status = "";
            this.OutputDirectoryPath = (String.IsNullOrWhiteSpace(Settings.Default.OutputDirectoryPath) ? GetTempDirectory() : Settings.Default.OutputDirectoryPath);
            this.LeftDirectoryPath = (String.IsNullOrWhiteSpace(Settings.Default.LeftDirectoryPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : Settings.Default.LeftDirectoryPath);
            this.RightDirectoryPath = (String.IsNullOrWhiteSpace(Settings.Default.RightDirectoryPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : Settings.Default.RightDirectoryPath);

            InitCommands();
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
                        this.leftFileInfos.AddRange(FilterFileInfosByExtension(leftDirectory, this.FileMask));
                    }
                }
                catch { }

                this.UpdateFileLists();
                this.SelectedFileIndex = (this.LeftFiles.Count > 0 && this.RightFiles.Count > 0 ? 0 : -1);
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
                        this.rightFileInfos.AddRange(FilterFileInfosByExtension(rightDirectory, this.FileMask));
                    }
                }
                catch { }

                this.UpdateFileLists();
                this.SelectedFileIndex = (this.LeftFiles.Count > 0 && this.RightFiles.Count > 0 ? 0 : -1);
                CommandManager.InvalidateRequerySuggested();
            }
            else if (propertyName == "SelectedFileIndex")
            {
                this.SelectedFileIndexIsInRange =
                    (this.SelectedFileIndex != -1
                    && this.SelectedFileIndex < this.LeftFiles.Count
                    && this.SelectedFileIndex < this.RightFiles.Count);

                CommandManager.InvalidateRequerySuggested();
            }
            else if (propertyName == "SelectedFileIndexIsInRange")
            {
                if (this.SelectedFileIndexIsInRange && this.CanUseLeftFile(null))
                {
                    this.left = this.LeftFiles[this.SelectedFileIndex];
                    FileInfo fi = this.left.FileInfo;
                    this.LeftFileSize = GetFormattedFileSize(ref fi);
                    this.LeftImagePath = this.left.FileInfo.FullName;
                }
                else
                {
                    this.left = null;
                    this.LeftImagePath = "";
                    this.LeftFileSize = "";
                }

                if (this.SelectedFileIndexIsInRange && this.CanUseRightFile(null))
                {
                    this.right = this.RightFiles[this.SelectedFileIndex];
                    FileInfo fi = this.right.FileInfo;
                    this.RightFileSize = GetFormattedFileSize(ref fi);
                    this.RightImagePath = this.right.FileInfo.FullName;
                }
                else
                {
                    this.right = null;
                    this.RightImagePath = "";
                    this.RightFileSize = "";
                }

                this.UpdateRecommendations();
            }
            else if (propertyName == "OutputDirectoryPath")
            {
                outputDirectory = new DirectoryInfo(this.OutputDirectoryPath);

                this.OutputDirectoryPathIsValid = (outputDirectory.Exists);

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

            if (this.left != null && this.right != null)
            {
                if (this.LeftImageSource != null && this.RightImageSource != null)
                {
                    if (this.LeftImageSource.Width > this.RightImageSource.Width
                        && this.LeftImageSource.Height > this.RightImageSource.Height)
                    {
                        this.IsLeftRecommended = true;
                    }
                    else if (this.LeftImageSource.Width < this.RightImageSource.Width
                        && this.LeftImageSource.Height < this.RightImageSource.Height)
                    {
                        this.IsRightRecommended = true;
                    }
                    else if (this.left.FileInfo.Length > this.right.FileInfo.Length)
                    {
                        this.IsLeftRecommended = true;
                    }
                    else if (this.left.FileInfo.Length < this.right.FileInfo.Length)
                    {
                        this.IsRightRecommended = true;
                    }
                }
            }
        }
        #endregion

        #region File selection
        public bool CanPreviousFile(object parameter)
        {
            return (this.SelectedFileIndex > 0);
        }

        public void PreviousFile(object parameter)
        {
            this.SelectedFileIndex = Math.Max(this.SelectedFileIndex - 1, 0);
        }

        public void PreviousMatchingFile(object parameter)
        {
            if (this.CanPreviousFile(parameter))
            {
                do
                {
                    this.PreviousFile(parameter);
                } while (this.CanPreviousFile(parameter) && this.right.IsUnique);
            }
        }

        public bool CanNextFile(object parameter)
        {
            return (this.SelectedFileIndex < Math.Min(this.LeftFiles.Count - 1, this.RightFiles.Count - 1));
        }

        public void NextFile(object parameter)
        {
            this.SelectedFileIndex = Math.Min(this.SelectedFileIndex + 1, Math.Min(this.LeftFiles.Count - 1, this.RightFiles.Count - 1));
        }

        public void NextMatchingFile(object parameter)
        {
            if (this.CanNextFile(parameter))
            {
                do
                {
                    this.NextFile(parameter);
                } while (this.CanNextFile(parameter) && this.left.IsUnique);
            }
        }

        public bool CanUseLeftFile(object parameter)
        {
            return (this.SelectedFileIndexIsInRange
                    && (!this.LeftFiles[this.SelectedFileIndex].IsEmpty)
                    && this.OutputDirectoryPathIsValid);
        }

        public void UseLeftFile(object parameter)
        {
            FileInfoWithCompareResult left = this.LeftFiles[this.SelectedFileIndex];

            UseFile(left);
        }

        public void UseLeftFileAndAdvance(object parameter)
        {
            this.UseLeftFile(parameter);
            this.NextFile(parameter);
        }

        public bool CanUseRightFile(object parameter)
        {
            return (this.SelectedFileIndexIsInRange
                    && (!this.RightFiles[this.SelectedFileIndex].IsEmpty)
                    && this.OutputDirectoryPathIsValid);
        }

        public void UseRightFile(object parameter)
        {
            FileInfoWithCompareResult right = this.RightFiles[this.SelectedFileIndex];

            UseFile(right);
        }

        public void UseRightFileAndAdvance(object parameter)
        {
            this.UseRightFile(parameter);
            this.NextFile(parameter);
        }

        private bool UseFile(FileInfoWithCompareResult file)
        {
            try
            {
                if (!file.IsEmpty
                    && file.FileInfo != null
                    && file.FileInfo.Exists)
                {
                    File.Copy(file.FileInfo.FullName, Path.Combine(outputDirectory.FullName, file.FileInfo.Name));

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

            DirectoryComparer directoryComparer = new DirectoryComparer(leftFileInfos, rightFileInfos, comparer);

            IEnumerable<FileInfo> leftUnique = directoryComparer.GetUniqueLeft();
            IEnumerable<FileInfo> rightUnique = directoryComparer.GetUniqueRight();

            this.LeftFiles.Clear();
            this.RightFiles.Clear();

            var left = leftFileInfos.GetEnumerator();
            var right = rightFileInfos.GetEnumerator();

            if (leftFileInfos.Count > 0 && rightFileInfos.Count > 0)
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
                } while (left.MoveNext());
            }

            if (right.Current != null)
            {
                do
                {
                    this.LeftFiles.Add(new FileInfoWithCompareResult());
                    this.RightFiles.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));
                } while (right.MoveNext());
            }
        }
        #endregion

        #endregion

        #region Properties
        private string status;
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

        private bool outputDirectoryPathIsValid;
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

        private ObservableCollection<FileInfoWithCompareResult> leftFiles = new ObservableCollection<FileInfoWithCompareResult>();
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

        private ObservableCollection<FileInfoWithCompareResult> rightFiles = new ObservableCollection<FileInfoWithCompareResult>();
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

        private int selectedFileIndex = 0;
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

        private bool selectedFileIndexIsInRange = false;
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

        string leftImagePath = "";
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

        string rightImagePath = "";
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

                    this.LeftImageDimensions = GetImageDimensions(ref image);
                }
                catch
                {
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

                    this.RightImageDimensions = GetImageDimensions(ref image);
                }
                catch
                {
                }

                return image;
            }
        }

        string leftImageDimensions = "-";
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

        string rightImageDimensions = "-";
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

        string leftFileSize = "";
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

        string rightFileSize = "";
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

        public bool isLeftRecommended;
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

        public bool isRightRecommended;
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
        RelayCommand previousFileCommand;
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

        RelayCommand nextFileCommand;
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
        RelayCommand previousMatchingFileCommand;
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

        RelayCommand nextFileMatchingCommand;
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

        RelayCommand useLeftFileCommand;
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

        RelayCommand useRightFileCommand;
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

        RelayCommand useLeftFileAndAdvanceCommand;
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

        RelayCommand useRightFileAndAdvanceCommand;
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

        private void InitCommands()
        {
            PreviousFileCommand = new RelayCommand(PreviousFile, CanPreviousFile);
            NextFileCommand = new RelayCommand(NextFile, CanNextFile);
            PreviousMatchingFileCommand = new RelayCommand(PreviousMatchingFile, CanPreviousFile);
            NextMatchingFileCommand = new RelayCommand(NextMatchingFile, CanNextFile);
            UseLeftFileCommand = new RelayCommand(UseLeftFile, CanUseLeftFile);
            UseRightFileCommand = new RelayCommand(UseRightFile, CanUseRightFile);
            UseLeftFileAndAdvanceCommand = new RelayCommand(UseLeftFileAndAdvance, CanUseLeftFile);
            UseRightFileAndAdvanceCommand = new RelayCommand(UseRightFileAndAdvance, CanUseRightFile);
        }
        #endregion

        #region The usual INPC implementation
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

            HandlePropertyChange(propertyName);
        }

        #endregion

        #region Misc
        public string GetTempDirectory()
        {
            string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }

        // From
        // http://stackoverflow.com/questions/20586/wpf-image-urisource-and-data-binding
        private static BitmapImage LoadImage(string fullPath)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnDemand;
            image.CreateOptions = BitmapCreateOptions.DelayCreation;
            image.UriSource = new Uri(fullPath, UriKind.Absolute);
            image.EndInit();

            // Used to avoid memory leaks?
            // http://blogs.msdn.com/b/jgoldb/archive/2008/05/04/memory-leaks-in-wpf-based-applications-blog-update.aspx
            image.Freeze();

            return image;
        }

        // From
        // http://stackoverflow.com/questions/128618/c-file-size-format-provider
        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

        // TODO: convert to extension method
        public string GetFormattedFileSize(ref FileInfo file)
        {
            StringBuilder buffer = new StringBuilder();
            StrFormatByteSize(file.Length, buffer, 100);
            return buffer.ToString();
        }

        // TODO: convert to extension method
        private string GetImageDimensions(ref BitmapImage image)
        {
            return String.Format("{0}x{1}px ({2}x{3} dpi)", image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY);
        }

        // TODO: create extension method
        private static IEnumerable<FileInfo> FilterFileInfosByExtension(DirectoryInfo leftDirectory, IEnumerable<string> extensions)
        {
            List<FileInfo> allFilesInFolder = new List<FileInfo>(leftDirectory.GetFiles());
            IEnumerable<FileInfo> filtered = allFilesInFolder.Where(fi => extensions.Contains(Path.GetExtension(fi.Name).ToLowerInvariant()));
            return filtered;
        }
        #endregion
    }
}
