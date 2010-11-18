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
                    this.RightFiles.Add(new FileInfoWithCompareResult(right.Current, leftUnique.Contains(right.Current)));
                } while (right.MoveNext());
            }
        }

        #region File selection
        public void PreviousFile()
        {
            this.SelectedFileIndex = Math.Max(this.SelectedFileIndex - 1, 0);
        }

        public void NextFile()
        {
            this.SelectedFileIndex = Math.Min(this.SelectedFileIndex + 1, Math.Min(this.LeftFiles.Count - 1, this.RightFiles.Count - 1));
        }

        public void UseLeftFile()
        {
            FileInfoWithCompareResult left = this.LeftFiles[this.SelectedFileIndex];

            UseFile(left);
        }

        public void UseRightFile()
        {
            FileInfoWithCompareResult right = this.RightFiles[this.SelectedFileIndex];

            UseFile(right);
        }

        private bool UseFile(FileInfoWithCompareResult file)
        {
            try
            {
                DirectoryInfo outputDirectory = new DirectoryInfo(this.OutputDirectoryPath);

                if (outputDirectory.Exists)
                {
                    if (!file.IsEmpty
                        && file.FileInfo != null
                        && file.FileInfo.Exists)
                    {
                        File.Copy(file.FileInfo.FullName, Path.Combine(outputDirectory.FullName, file.FileInfo.Name));

                        return true;
                    }
                }
            }
            catch
            {
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
        #endregion

        #region Commands
        DelegateCommand previousFileCommand;
        public DelegateCommand PreviousFileCommand
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

        DelegateCommand nextFileCommand;
        public DelegateCommand NextFileCommand
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

        DelegateCommand useLeftFileCommand;
        public DelegateCommand UseLeftFileCommand
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

        DelegateCommand useRightFileCommand;
        public DelegateCommand UseRightFileCommand
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
            PreviousFileCommand = new DelegateCommand(PreviousFile);
            NextFileCommand = new DelegateCommand(NextFile);
            UseLeftFileCommand = new DelegateCommand(UseLeftFile);
            UseRightFileCommand = new DelegateCommand(UseRightFile);
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
