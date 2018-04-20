using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QuizLight.View
{
    /// <summary>
    /// Converts:
    ///     true -> FontWeights.Bold
    ///     false -> FontWeights.Normal
    /// </summary>
    public class BooleanToWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
