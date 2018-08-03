using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace QueryTabControl
{
    /// <summary>
    /// A content for TabControl.
    ///     QueryName is usually a uppercase name of SQL command.
    ///     Data is DataTable of records.
    /// </summary>
    public class TabContent : INotifyPropertyChanged
    {
        private string queryName;
        public string QueryName
        {
            get => queryName;
            set => Set(ref queryName, value);
        }

        private DataTable data;
        public DataTable Data
        {
            get => data;
            set => Set(ref data, value);
        }

        public TabContent(string queryName, DataTable data)
        {
            QueryName = queryName;
            Data = data;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool Set<T>(ref T oldValue, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(oldValue, value))
            {
                return false;
            }
            else
            {
                oldValue = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        #endregion

    }
}