namespace WellCompareDir.WPF.Library.Extensions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class DirectoryInfoHelper
    {
        public static IEnumerable<FileInfo> FilterFileInfosByExtension(this DirectoryInfo directory, IEnumerable<string> extensions)
        {
            List<FileInfo> allFilesInFolder = new List<FileInfo>(directory.GetFiles());
            IEnumerable<FileInfo> filtered = allFilesInFolder.Where(fi => extensions.Contains(Path.GetExtension(fi.Name).ToLowerInvariant()));
            return filtered;
        }
    }
}
