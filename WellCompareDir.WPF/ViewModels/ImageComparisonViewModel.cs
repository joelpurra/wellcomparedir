using WellCompareDir.WPF.ViewModels;
namespace WellCompareDir.WPF
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using WellCompareDir.Comparer;
    using WellCompareDir.WPF.Library;
    using WellCompareDir.WPF.Library.Extensions;
    using WellCompareDir.WPF.Library.Helpers;

    public class ImageComparisonViewModel : ViewModelBase
    {
        private ObservableCollection<FileInfoWithCompareResult> files;
        private FileInfoWithCompareResult selectedFile;
        private string imagePath;
        private string directoryPath;
        private string imageDimensions;
        private string fileSize;
        private bool isRecommended;
        private int selectedFileIndex;
        private bool selectedFileIndexIsInRange;

        private RelayCommand removeComparisonCommand;
        private RelayCommand useFileCommand;
        private RelayCommand useFileAndAdvanceCommand;

        public ImageComparisonViewModel()
        {
            //string name
            //this.Name = name;
            this.Name = Guid.NewGuid().ToString();

            this.FileInfos = new List<FileInfo>();
            this.Files = new ObservableCollection<FileInfoWithCompareResult>();
            this.ImagePath = string.Empty;
            this.ImageDimensions = "-";
            this.FileSize = string.Empty;
            this.DirectoryPath = null;
            this.IsRecommended = false;
        }

        public string Name { get; private set; }

        public List<FileInfo> FileInfos { get; set; }

        public ObservableCollection<FileInfoWithCompareResult> Files
        {
            get
            {
                return this.files;
            }

            set
            {
                this.files = value;
                this.OnPropertyChanged("Files");
            }
        }

        public FileInfoWithCompareResult SelectedFile
        {
            get
            {
                return this.selectedFile;
            }

            set
            {
                if (value != this.selectedFile)
                {
                    this.selectedFile = value;
                    this.OnPropertyChanged("SelectedFile");
                }
            }
        }

        public string ImagePath
        {
            get
            {
                return this.imagePath;
            }

            set
            {
                if (value != this.imagePath)
                {
                    this.imagePath = value;
                    this.OnPropertyChanged("ImagePath");
                }
            }
        }

        public BitmapImage ImageSource
        {
            get
            {
                BitmapImage image = null;

                this.imageDimensions = "-";

                try
                {
                    image = LoadImage(this.imagePath);

                    this.ImageDimensions = image.GetImageDimensions();
                }
                catch
                {
                    // TODO: don't hide errrrrrrors
                }

                return image;
            }
        }

        public string ImageDimensions
        {
            get
            {
                return this.imageDimensions;
            }

            set
            {
                this.imageDimensions = value;
                this.OnPropertyChanged("ImageDimensions");
            }
        }

        public string FileSize
        {
            get
            {
                return this.fileSize;
            }

            set
            {
                this.fileSize = value;
                this.OnPropertyChanged("FileSize");
            }
        }

        public string DirectoryPath
        {
            get
            {
                return this.directoryPath;
            }

            set
            {
                this.directoryPath = value;
                this.OnPropertyChanged("DirectoryPath");
            }
        }

        public bool IsRecommended
        {
            get
            {
                return this.isRecommended;
            }

            set
            {
                this.isRecommended = value;
                this.OnPropertyChanged("IsRecommended");
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

        public RelayCommand RemoveComparisonCommand
        {
            get
            {
                return this.removeComparisonCommand;
            }

            set
            {
                this.removeComparisonCommand = value;
                this.OnPropertyChanged("RemoveComparisonCommand");
            }
        }


        public RelayCommand UseFileCommand
        {
            get
            {
                return this.useFileCommand;
            }

            set
            {
                this.useFileCommand = value;
                this.OnPropertyChanged("UseFileCommand");
            }
        }

        public RelayCommand UseFileAndAdvanceCommand
        {
            get
            {
                return this.useFileAndAdvanceCommand;
            }

            set
            {
                this.useFileAndAdvanceCommand = value;
                this.OnPropertyChanged("UseFileAndAdvanceCommand");
            }
        }

        public void RemoveComparison(object parameter = null)
        {
            throw new NotImplementedException();
        }

        public bool CanUseFile(object parameter = null)
        {
            return this.SelectedFileIndexIsInRange
                       && (!this.Files[this.SelectedFileIndex].IsEmpty);
        }

        public void UseFile(object parameter = null)
        {
            this.UseFileCommand.Execute(this.SelectedFile);
        }

        public void UseFileAndAdvance(object parameter = null)
        {
            this.UseFileAndAdvanceCommand.Execute(this.SelectedFile);
        }

        protected override void HandlePropertyChange(string propertyName)
        {
            if (propertyName == "DirectoryPath")
            {
                this.FileInfos.Clear();
                this.SelectedFileIndex = -1;

                try
                {
                    DirectoryInfo directory = new DirectoryInfo(this.directoryPath);

                    if (directory.Exists)
                    {
                        this.FileInfos.AddRange(directory.FilterFileInfosByExtension(ImageFileTypes.ExtensionMask));
                    }
                }
                catch
                {
                    // TODO: more logging
                }

                // TODO: actually update files
                //this.PARENTOBJECT.UpdateFileLists();
            }
            else if (propertyName == "SelectedFileIndex")
            {
                this.SelectedFileIndexIsInRange =
                    this.SelectedFileIndex != -1
                    && this.SelectedFileIndex < this.Files.Count;

                CommandManager.InvalidateRequerySuggested();
            }
            else if (propertyName == "SelectedFileIndexIsInRange")
            {
                this.SelectedFile = this.GetFileInfoWithCompareResult(this.SelectedFileIndex);
            }
            else if (propertyName == "SelectedFile")
            {
                if (this.SelectedFile != null)
                {
                    FileInfo fi = this.SelectedFile.FileInfo;
                    this.FileSize = fi.GetFormattedFileSize();
                    this.ImagePath = this.SelectedFile.FileInfo.FullName;
                }
                else
                {
                    this.ImagePath = string.Empty;
                    this.FileSize = string.Empty;
                }

                // TODO: actually update files
                //this.PARENTOBJECT.UpdateRecommendations();
            }
            else if (propertyName == "ImagePath")
            {
                this.OnPropertyChanged("ImageSource");

                // TODO: actually update files
                //this.PARENTOBJECT.UpdateRecommendations();
            }
        }

        public FileInfoWithCompareResult GetFileInfoWithCompareResult(int index)
        {
            if (!this.CanUseFile())
            {
                return null;
            }

            return this.Files[index];
        }

        // From
        // http://stackoverflow.com/questions/20586/wpf-image-urisource-and-data-binding
        // TODO: change to FileInfo?
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
            // http://blogs.msdn.com/b/jgoldb/archive/2008/05/04/memory-leaks-in-wpf-based-applications-blog-update.aspx
            if (image.CanFreeze)
            {
                image.Freeze();
            }

            return image;
        }

        private void InitCommands()
        {
            this.UseFileCommand = new RelayCommand(this.UseFile, this.CanUseFile);
            this.UseFileAndAdvanceCommand = new RelayCommand(this.UseFileAndAdvance, this.CanUseFile);

            this.RemoveComparisonCommand = new RelayCommand(this.RemoveComparison);
        }
    }
}
