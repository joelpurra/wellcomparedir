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
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();

            leftImage.SizeChanged += new SizeChangedEventHandler(image_SizeChanged);
            rightImage.SizeChanged += new SizeChangedEventHandler(image_SizeChanged);
        }

        void image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetStretchMode(sender);
        }

        private void SetStretchMode(object sender)
        {
            Image image = sender as Image;

            if (image != null)
            {
                BitmapSource source = (BitmapSource)image.Source;

                if (source != null)
                {
                    double WPF_DPI_BASE = 96;

                    double imageScaleX = source.DpiX / WPF_DPI_BASE;
                    double imageScaleY = source.DpiY / WPF_DPI_BASE;

                    image.RenderTransform = new ScaleTransform((imageScaleX / this.ScreenScale.ScaleX), (imageScaleY / this.ScreenScale.ScaleY));

                    //FrameworkElement parent = image.Parent as FrameworkElement;

                    //if (source.PixelWidth < parent.ActualWidth
                    //    || source.PixelHeight < parent.ActualHeight)
                    //{
                    //    image.Stretch = Stretch.None;
                    //}
                    //else
                    //{
                    //    image.Stretch = Stretch.Uniform;
                    //}
                }
                else
                {
                    image.RenderTransform = Transform.Identity;
                }
            }
        }

        private ScaleTransform screenScale = null;
        private ScaleTransform ScreenScale
        {
            get
            {
                if (screenScale == null)
                {
                    Matrix m = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice;
                    double screenScaleX = m.M11;
                    double screenScaleY = m.M22;

                    screenScale = new ScaleTransform(screenScaleX, screenScaleY);
                }

                return screenScale;
            }
        }
    }
}
