using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ImageSelectionInfo.View
{
    /// <summary>
    /// Loads image, based on input string.
    /// </summary>
    public class ImageToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Uri uri = new Uri(value as string);
                BitmapImage bitmap = new BitmapImage(uri);
                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}