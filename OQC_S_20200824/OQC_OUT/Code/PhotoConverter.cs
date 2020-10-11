using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace OQC_OUT
{
    public class PhotoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string photo = value?.ToString();
            string photoBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Photo");
            if (!Directory.Exists(photoBasePath))
                return null;
            string path = Path.Combine(photoBasePath, string.IsNullOrEmpty(photo) ? "no.jpg" : photo);
            if (!File.Exists(path))
                path = Path.Combine(photoBasePath, "no.jpg");
            
            //return new BitmapImage(new Uri(path, UriKind.Absolute));

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            using (Stream sm = new MemoryStream(File.ReadAllBytes(path)))
            {
                
                bitmapImage.StreamSource = sm;
                //bitmapImage.UriSource = new Uri(path);
                bitmapImage.DecodePixelWidth = 100;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
