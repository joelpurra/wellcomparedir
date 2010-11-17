using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WellCompareDir.Comparer
{
    // Parts from
    // http://msdn.microsoft.com/en-us/library/bb546137.aspx

    // This implementation defines a very simple comparison
    // between two FileInfo objects. It only compares the name
    // of the files being compared and their length in bytes.
    public class SimpleNameComparer : IEqualityComparer<FileInfo>
    {
        public SimpleNameComparer() { }

        public bool Equals(FileInfo f1, FileInfo f2)
        {
            return (f1.Name == f2.Name &&
                    f1.Length == f2.Length);
        }

        // Return a hash that reflects the comparison criteria. According to the 
        // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must
        // also be equal. Because equality as defined here is a simple value equality, not
        // reference identity, it is possible that two or more objects will produce the same
        // hash code.
        public int GetHashCode(FileInfo fi)
        {
            string s = String.Format("{0}{1}", fi.Name, fi.Length);
            return s.GetHashCode();
        }
    }
}
