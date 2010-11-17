namespace WellCompareDir.WPF
{
    using System.Windows;
    using Framework;

    public partial class App : Application
    {
        public App()
        {
            IoCBuilder.CollectViewAndViewModelMappings();
        }
    }
}
