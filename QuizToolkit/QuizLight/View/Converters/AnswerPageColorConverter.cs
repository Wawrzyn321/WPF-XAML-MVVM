using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace QuizLight.View
{
    /// <summary>
    /// Converter for different colors
    /// on odd and even question pages.
    /// </summary>
    public class AnswerPageColorConverter : IValueConverter
    {
        private static readonly SolidColorBrush evenBrush;
        private static readonly SolidColorBrush oddBrush;

        static AnswerPageColorConverter()
        {
            evenBrush = Application.Current.Resources["secondaryColorBrush"] as SolidColorBrush;
            oddBrush = Application.Current.Resources["secondaryColorDarkerBrush"] as SolidColorBrush;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            if (listView != null && item != null)
            {
                int index = listView.ItemContainerGenerator.IndexFromContainer(item);
                return index % 2 == 0 ? evenBrush : oddBrush;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}