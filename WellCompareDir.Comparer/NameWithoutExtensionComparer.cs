using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WellCompareDir.Comparer
{
    // Parts from
    // http://msdn.microsoft.com/en-us/library/bb546137.aspx

    // This implementation defines a simple comparison
    // between two FileInfo objects. It only compares the name
    // of the files (excluding extension) being compared and their length in bytes.
    public class NameWithoutExtensionComparer : IEqualityComparer<FileInfo>, IComparer<FileInfo>
    {
        public NameWithoutExtensionComparer() { }

        public bool Equals(FileInfo x, FileInfo y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null)
            {
                return false;
            }

            if (y == null)
            {
                return false;
            }

            return (this.Compare(x, y) == 0);
        }

        // Return a hash that reflects the comparison criteria. According to the 
        // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must
        // also be equal. Because equality as defined here is a simple value equality, not
        // reference identity, it is possible that two or more objects will produce the same
        // hash code.
        public int GetHashCode(FileInfo fi)
        {
            string withoutExtension = Path.GetFileNameWithoutExtension(fi.Name).ToLowerInvariant();

            return withoutExtension.GetHashCode();
        }

        #region IComparer<FileInfo> Members

        public int Compare(FileInfo x, FileInfo y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return 1;
            }

            if (y == null)
            {
                return -1;
            }

            string withoutExtensionX = Path.GetFileNameWithoutExtension(x.Name).ToLowerInvariant();
            string withoutExtensionY = Path.GetFileNameWithoutExtension(y.Name).ToLowerInvariant();

            return withoutExtensionX.CompareTo(withoutExtensionY);
        }

        #endregion
    }
}
