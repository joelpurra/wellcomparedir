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
using System.Windows.Forms;
using WellCompareDir.WPF.Properties;

namespace WellCompareDir.WPF
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        DirectoryInfo outputDirectory = null;

        List<FileInfo> leftFileInfos = new List<FileInfo>();
        List<FileInfo> rightFileInfos = new List<FileInfo>();

        public MainWindowViewModel()
        {
            this.Status = "";
            this.OutputDirectoryPath = (String.IsNullOrWhiteSpace(Settings.Default.OutputDirectoryPath) ? GetTempDirectory() : Settings.Default.OutputDirectoryPath);
            this.LeftDirectoryPath = (String.IsNullOrWhiteSpace(Settings.Default.LeftDirectoryPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : Settings.Default.LeftDirectoryPath);
            this.RightDirectoryPath = (String.IsNullOrWhiteSpace(Settings.Default.RightDirectoryPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : Settings.Default.RightDirectoryPath);

            InitCommands();
        }

        ~MainWindowViewModel()
        {
            Settings.Default.OutputDirectoryPath = this.OutputDirectoryPath;
            Settings.Default.LeftDirectoryPath = this.LeftDirectoryPath;
            Settings.Default.RightDirectoryPath = this.RightDirectoryPath;
            Settings.Default.Save();
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
                        this.leftFileInfos.AddRange(leftDirectory.GetFiles());
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
                        this.rightFileInfos.AddRange(rightDirectory.GetFiles());
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
                    FileInfo left = this.LeftFiles[this.SelectedFileIndex].FileInfo;
                    this.LeftFileSize = GetFormattedFileSize(ref left);
                    this.LeftImagePath = left.FullName;
                }
                else
                {
                    this.LeftImagePath = "";
                    this.LeftFileSize = "";
                }

                if (this.SelectedFileIndexIsInRange && this.CanUseRightFile(null))
                {
                    FileInfo right = this.RightFiles[this.SelectedFileIndex].FileInfo;
                    this.RightFileSize = GetFormattedFileSize(ref right);
                    this.RightImagePath = right.FullName;
                }
                else
                {
                    this.RightImagePath = "";
                    this.RightFileSize = "";
                }
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
            }
            else if (propertyName == "RightImagePath")
            {
                this.OnPropertyChanged("RightImageSource");
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

        public bool CanNextFile(object parameter)
        {
            return (this.SelectedFileIndex < Math.Min(this.LeftFiles.Count - 1, this.RightFiles.Count - 1));
        }

        public void NextFile(object parameter)
        {
            this.SelectedFileIndex = Math.Min(this.SelectedFileIndex + 1, Math.Min(this.LeftFiles.Count - 1, this.RightFiles.Count - 1));
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
            IEqualityComparer<FileInfo> comparer = new SimpleNameComparer();

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
                    int cmpOuter = left.Current.Name.CompareTo(right.Current.Name);

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

                        while (right.MoveNext())
                        {
                            int cmpInner = left.Current.Name.CompareTo(right.Current.Name);

                            if (cmpInner > 0)
                            {
                                this.LeftFiles.Add(new FileInfoWithCompareResult());
                                this.RightFiles.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));
                            }
                            else if (cmpInner == 0)
                            {
                                this.LeftFiles.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                                this.RightFiles.Add(new FileInfoWithCompareResult(right.Current, rightUnique.Contains(right.Current)));

                                right.MoveNext();
                                break;
                            }
                            else
                            {
                                this.LeftFiles.Add(new FileInfoWithCompareResult(left.Current, leftUnique.Contains(left.Current)));
                                this.RightFiles.Add(new FileInfoWithCompareResult());

                                break;
                            }
                        }

                        if (right.Current == null)
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

        private string outputDirectoryPath;
        public string OutputDirectoryPath
        {
            get
            {
                return this.outputDirectoryPath;
            }
            set
            {
                this.outputDirectoryPath = value;
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

        private string leftDirectoryPath;
        public string LeftDirectoryPath
        {
            get
            {
                return this.leftDirectoryPath;
            }
            set
            {
                this.leftDirectoryPath = value;
                this.OnPropertyChanged("LeftDirectoryPath");
            }
        }

        private string rightDirectoryPath;
        public string RightDirectoryPath
        {
            get
            {
                return this.rightDirectoryPath;
            }
            set
            {
                this.rightDirectoryPath = value;
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

        public object LeftImageSource
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

        public object RightImageSource
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
                this.OnPropertyChanged("Previous");
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

        RelayCommand browseForOutputDirectoryCommand;
        public RelayCommand BrowseForOutputDirectoryCommand
        {
            get
            {
                return this.browseForOutputDirectoryCommand;
            }
            set
            {
                this.browseForOutputDirectoryCommand = value;
                this.OnPropertyChanged("BrowseForOutputDirectoryCommand");
            }
        }

        RelayCommand browseForLeftDirectoryCommand;
        public RelayCommand BrowseForLeftDirectoryCommand
        {
            get
            {
                return this.browseForLeftDirectoryCommand;
            }
            set
            {
                this.browseForLeftDirectoryCommand = value;
                this.OnPropertyChanged("BrowseForLeftDirectoryCommand");
            }
        }

        RelayCommand browseForRightDirectoryCommand;
        public RelayCommand BrowseForRightDirectoryCommand
        {
            get
            {
                return this.browseForRightDirectoryCommand;
            }
            set
            {
                this.browseForRightDirectoryCommand = value;
                this.OnPropertyChanged("BrowseForRightDirectoryCommand");
            }
        }

        private void InitCommands()
        {
            PreviousFileCommand = new RelayCommand(PreviousFile, CanPreviousFile);
            NextFileCommand = new RelayCommand(NextFile, CanNextFile);
            UseLeftFileCommand = new RelayCommand(UseLeftFile, CanUseLeftFile);
            UseRightFileCommand = new RelayCommand(UseRightFile, CanUseRightFile);
            BrowseForOutputDirectoryCommand = new RelayCommand(BrowseForOutputDirectory);
            BrowseForLeftDirectoryCommand = new RelayCommand(BrowseForLeftDirectory);
            BrowseForRightDirectoryCommand = new RelayCommand(BrowseForRightDirectory);
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

        public string BrowseForFolder(string description, string start)
        {
            string selected = null;

            FolderBrowserDialog dlg = new FolderBrowserDialog();

            dlg.Description = description;

            dlg.SelectedPath = start;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                selected = dlg.SelectedPath;
            }

            return selected;
        }

        public void BrowseForOutputDirectory(object parameter)
        {
            this.OutputDirectoryPath = this.BrowseForFolder("Select output directory path", this.OutputDirectoryPath) ?? this.OutputDirectoryPath;
        }

        public void BrowseForLeftDirectory(object parameter)
        {
            this.LeftDirectoryPath = this.BrowseForFolder("Select left directory path", this.LeftDirectoryPath) ?? this.LeftDirectoryPath;
        }

        public void BrowseForRightDirectory(object parameter)
        {
            this.RightDirectoryPath = this.BrowseForFolder("Select right directory path", this.RightDirectoryPath) ?? this.RightDirectoryPath;
        }
        #endregion
    }
}
