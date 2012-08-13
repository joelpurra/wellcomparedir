namespace WellCompareDir.WPF.Library.Helpers
{
    using System.Windows.Forms;


    public static class FolderHelper
    {
        public static string BrowseForFolder(string description, string start)
        {
            string selected = null;

            FolderBrowserDialog dlg = new FolderBrowserDialog();

            dlg.Description = description;

            dlg.SelectedPath = start;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selected = dlg.SelectedPath;
            }

            return selected;
        }
    }
}
