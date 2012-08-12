namespace WellCompareDir.Comparer
{
    using System.IO;

    /// <summary>
    ///     File info paired with comparison results.
    /// </summary>
    public class FileInfoWithCompareResult
    {
        public FileInfoWithCompareResult()
            : this(true, null, false)
        {
        }

        public FileInfoWithCompareResult(FileInfo fileInfo, bool isUnique)
            : this(false, fileInfo, isUnique)
        {
        }

        public FileInfoWithCompareResult(bool isEmpty, FileInfo fileInfo, bool isUnique)
        {
            this.IsEmpty = isEmpty;
            this.FileInfo = fileInfo;

            this.DisplayName = this.FileInfo == null ? string.Empty : this.FileInfo.Name ?? "(null)";

            this.IsUnique = isUnique;
        }

        public bool IsEmpty { get; set; }

        public FileInfo FileInfo { get; set; }

        public string DisplayName { get; set; }

        public bool IsUnique { get; set; }

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}
