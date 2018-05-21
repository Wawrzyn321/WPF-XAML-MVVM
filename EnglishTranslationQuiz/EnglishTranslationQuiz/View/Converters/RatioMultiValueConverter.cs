using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace MVVMTest2.View.Converters
{
    /// <summary>
    /// Converter for UI displaying ratio of:
    ///     - learned words and all words in given set.
    ///     - loaded file index and all files to load.
    /// </summary>
    public class RatioMultiValueConverter : IMultiValueConverter
    {
        //values[0] is current item index
        //values[1] is count of all items

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{values[0]} / {values[1]}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
