﻿namespace WellCompareDir.WPF
{
    using System.Windows;
    using Framework;
    using WellCompareDir.WPF.Properties;

    public partial class App : Application
    {
        public App()
        {
            IoCBuilder.CollectViewAndViewModelMappings();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Settings.Default.Save();
        }
    }
}
