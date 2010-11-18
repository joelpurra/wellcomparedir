using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using WellCompareDir.Comparer;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace WellCompareDir.WPF
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        DirectoryInfo outputDirectory = null;

        public MainWindowViewModel()
        {
            //// Create two identical or different temporary folders 
            //// on a local drive and change these file paths.
            //string pathA = @"C:\TestDir";
            //string pathB = @"C:\TestDir2";

            //DirectoryInfo dir1 = new DirectoryInfo(pathA);
            //DirectoryInfo dir2 = new DirectoryInfo(pathB);

            //// Take a snapshot of the file system.
            //IEnumerable<FileInfo> list1 = dir1.GetFiles("*.*", SearchOption.AllDirectories);
            //IEnumerable<FileInfo> list2 = dir2.GetFiles("*.*", SearchOption.AllDirectories);

            this.Status = "";
            this.OutputDirectoryPath = GetTempDirectory();
            this.LeftDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            this.RightDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            InitCommands();
        }

        public string GetTempDirectory()
        {
            string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }

        List<FileInfo> leftFileInfos = new List<FileInfo>();
        List<FileInfo> rightFileInfos = new List<FileInfo>();

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
                    this.LeftImagePath = this.LeftFiles[this.SelectedFileIndex].FileInfo.FullName;
                }
                else
                {
                    this.LeftImagePath = "";
                }

                if (this.SelectedFileIndexIsInRange && this.CanUseRightFile(null))
                {
                    this.RightImagePath = this.RightFiles[this.SelectedFileIndex].FileInfo.FullName;
                }
                else
                {
                    this.RightImagePath = "";
                }
            }
            else if (propertyName == "OutputDirectoryPath")
            {
                outputDirectory = new DirectoryInfo(this.OutputDirectoryPath);

                this.OutputDirectoryPathIsValid = (outputDirectory.Exists);

                CommandManager.InvalidateRequerySuggested();
            }
        }

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
                this.leftImagePath = value;
                this.OnPropertyChanged("LeftImagePath");
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
                this.rightImagePath = value;
                this.OnPropertyChanged("RightImagePath");
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

        private void InitCommands()
        {
            PreviousFileCommand = new RelayCommand(PreviousFile, CanPreviousFile);
            NextFileCommand = new RelayCommand(NextFile, CanNextFile);
            UseLeftFileCommand = new RelayCommand(UseLeftFile, CanUseLeftFile);
            UseRightFileCommand = new RelayCommand(UseRightFile, CanUseRightFile);
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
    }
}
