namespace WellCompareDir.WPF.Library.Extensions
{
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class FileInfoHelper
    {
        // From
        // http://stackoverflow.com/questions/128618/c-file-size-format-provider
        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        private static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

        public static string GetFormattedFileSize(this FileInfo file)
        {
            StringBuilder buffer = new StringBuilder();

            StrFormatByteSize(file.Length, buffer, 100);

            return buffer.ToString();
        }
    }
}
