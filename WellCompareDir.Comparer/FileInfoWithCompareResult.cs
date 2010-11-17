using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WellCompareDir.Comparer
{
    public class FileInfoWithCompareResult
    {
        public bool IsEmpty { get; set; }
        public FileInfo FileInfo { get; set; }
        public string DisplayName { get; set; }
        public bool IsUnique { get; set; }

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

            this.DisplayName = (this.FileInfo == null ? "" : this.FileInfo.Name ?? "(null)");

            this.IsUnique = isUnique;
        }

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}
