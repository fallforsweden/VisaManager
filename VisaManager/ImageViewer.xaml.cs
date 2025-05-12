using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VisaManager
{
    public partial class ImageViewer : Window
    {
        public ImageViewer(string imagePath)
        {
            InitializeComponent();
            if (System.IO.File.Exists(imagePath))
            {
                FullImage.Source = new BitmapImage(new Uri(imagePath));
            }
        }
    }
}
