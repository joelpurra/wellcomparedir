namespace WellCompareDir.WPF.Library.Extensions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class DirectoryInfoExtensions
    {
        public static IEnumerable<FileInfo> FilterFileInfosByExtension(DirectoryInfo leftDirectory, IEnumerable<string> extensions)
        {
            List<FileInfo> allFilesInFolder = new List<FileInfo>(leftDirectory.GetFiles());
            IEnumerable<FileInfo> filtered = allFilesInFolder.Where(fi => extensions.Contains(Path.GetExtension(fi.Name).ToLowerInvariant()));
            return filtered;
        }
    }
}
