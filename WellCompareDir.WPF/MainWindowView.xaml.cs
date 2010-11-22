using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WellCompareDir.WPF
{
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();

            InitCommands();
        }

        private void CanOpenAboutWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenAboutWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Window aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        public static RoutedUICommand OpenAboutWindowCommand = new RoutedUICommand();

        private void InitCommands()
        {
            CommandBinding openAboutWindowBinding = new CommandBinding(OpenAboutWindowCommand, OpenAboutWindow, CanOpenAboutWindow);

            this.CommandBindings.Add(openAboutWindowBinding);
        }

        private void leftFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.useLeftButton != null)
            {
                this.useLeftButton.Focus();
            }
        }

        private void rightFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.useRightButton != null)
            {
                this.useRightButton.Focus();
            }
        }
    }
}
