using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFSQLPad.View
{

    /// <summary>
    /// Converts true to light green color, false to red
    /// </summary>
    public class AvailabilityToColorConverter : IValueConverter
    {
        private readonly SolidColorBrush brushAvailable = new SolidColorBrush(Colors.Lime);
        private readonly SolidColorBrush brushUnavailable = new SolidColorBrush(Colors.Red);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                Debug.WriteLine("AvailabilityToColorConverter: null value!");
                return brushUnavailable;
            }

            return (bool) value ? brushAvailable : brushUnavailable;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
