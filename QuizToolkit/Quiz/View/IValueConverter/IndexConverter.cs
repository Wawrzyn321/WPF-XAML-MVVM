using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Quiz.View
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            if (listView != null && item != null)
            {
                int index = listView.ItemContainerGenerator.IndexFromContainer(item);
                return $"Question #{(index + 1)}";
            }
            throw new Exception("kurwa");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}
