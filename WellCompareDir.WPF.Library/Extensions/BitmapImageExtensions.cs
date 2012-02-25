namespace WellCompareDir.WPF.Library.Extensions
{
    using System.Windows.Media.Imaging;

    public static class BitmapImageExtensions
    {
        public static long Area(this BitmapImage bmp)
        {
            return bmp.PixelWidth * bmp.PixelHeight;
        }

        public static string GetImageDimensions(this BitmapImage image)
        {
            return string.Format("{0}x{1}px ({2}x{3} dpi)", image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY);
        }
    }
}
