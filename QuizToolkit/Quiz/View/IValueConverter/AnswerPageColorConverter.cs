using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Quiz.View
{
    public class AnswerPageColorConverter : IValueConverter
    {
        private static SolidColorBrush evenBrush;
        private static SolidColorBrush oddBrush;

        static AnswerPageColorConverter()
        {
            evenBrush = Application.Current.Resources["secondaryColorBrush"] as SolidColorBrush;
            oddBrush = Application.Current.Resources["secondaryColorDarkerBrush"] as SolidColorBrush;
            Debug.Assert(evenBrush != null && oddBrush != null);
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
            throw new Exception("kurwa5");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}