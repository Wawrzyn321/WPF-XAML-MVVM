using System;
using System.Globalization;
using System.Windows.Data;

namespace JakiTydzienApp
{
    /// <summary>
    /// Converts the given string to uppercase.
    /// </summary>
    public class StringCapitalizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            string str = (string) value;
            return str.ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
