namespace WellCompareDir.Comparer
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    ///     Parts from
    ///     https://msdn.microsoft.com/en-us/library/bb546137.aspx
    ///     This implementation defines a very simple comparison
    ///     between two FileInfo objects. It only compares the name
    ///     of the files being compared.
    /// </summary>
    public class SimpleNameComparer : IEqualityComparer<FileInfo>
    {
        public SimpleNameComparer()
        {
        }

        public bool Equals(FileInfo f1, FileInfo f2)
        {
            return f1.Name.ToLowerInvariant() == f2.Name.ToLowerInvariant();
        }

        /// <summary>
        /// Return a hash that reflects the comparison criteria. According to the
        /// rules for IEqualityComparer<T>, if Equals is true, then the hash codes must
        /// also be equal. Because equality as defined here is a simple value equality, not
        /// reference identity, it is possible that two or more objects will produce the same
        /// hash code.
        /// </summary>
        public int GetHashCode(FileInfo fi)
        {
            return fi.Name.ToLowerInvariant().GetHashCode();
        }
    }
}
