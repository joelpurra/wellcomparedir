namespace WellCompareDir.Comparer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    ///     Compare two directories, left and right.
    ///     Parts from http://msdn.microsoft.com/en-us/library/bb546137.aspx.
    /// </summary>
    public class DirectoryComparer
    {
        public DirectoryComparer(IEnumerable<FileInfo> left, IEnumerable<FileInfo> right, IEqualityComparer<FileInfo> comparer)
        {
            this.Left = left;
            this.Right = right;
            this.Comparer = comparer;
        }

        public IEnumerable<FileInfo> Left { get; private set; }

        public IEnumerable<FileInfo> Right { get; private set; }

        public IEqualityComparer<FileInfo> Comparer { get; private set; }

        // TODO: implement IEqualityComparer<IList<FileInfo>>
        public bool AreIdentical()
        {
            // This query determines whether the two folders contain
            // identical file lists, based on the custom file comparer
            // that is defined in the FileCompare class.
            // The query executes immediately because it returns a bool.
            return this.Left.SequenceEqual(this.Right, this.Comparer);
        }

        public IEnumerable<FileInfo> GetSmililarities()
        {
            // Find the common files. It produces a sequence and doesn't 
            // execute until the foreach statement.
            return this.Left.Intersect(this.Right, this.Comparer);
        }

        public IEnumerable<FileInfo> GetUniqueLeft()
        {
            var leftOnly = (from file in this.Left
                            select file).Except(this.Right, this.Comparer);

            return leftOnly;
        }

        public IEnumerable<FileInfo> GetUniqueRight()
        {
            var rightOnly = (from file in this.Right
                             select file).Except(this.Left, this.Comparer);

            return rightOnly;
        }

        public IEnumerable<FileInfo> GetDifferences()
        {
            // Find the set difference between the two folders.
            var differences = this.GetUniqueLeft()
                                  .Concat(this.GetUniqueRight());

            return differences;
        }
    }
}
