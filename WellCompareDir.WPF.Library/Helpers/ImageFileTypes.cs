namespace WellCompareDir.WPF.Library.Helpers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public static class ImageFileTypes
    {
        private static List<string> extensionMask;

        static ImageFileTypes()
        {
            InitializeKnownImageFiletypeExtensions();
        }

        public static ReadOnlyCollection<string> ExtensionMask
        {
            get
            {
                return extensionMask.AsReadOnly();
            }
        }

        private static void InitializeKnownImageFiletypeExtensions()
        {
            // TODO: let user add more in through App.config?
            extensionMask = new List<string>
            {
                ".bmp",
                ".jpg",
                ".jpeg",
                ".jpe",
                ".png",
                ".tif",
                ".tiff",
                ".hdp",
                ".wdp",
                ".jxr",
                ".gif",
                ".ico",
            };
        }
    }
}
