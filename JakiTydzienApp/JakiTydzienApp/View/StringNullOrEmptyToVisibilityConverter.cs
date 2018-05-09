using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace JakiTydzienApp
{
    /// <summary>
    /// Set visibility to Collapsed when the WeekDetails are not provided.
    /// </summary>
    public class StringNullOrEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string) value;
            bool shouldHideText = string.IsNullOrEmpty(str);
            return shouldHideText ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
