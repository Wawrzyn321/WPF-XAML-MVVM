using System.Data;
using Model;

namespace MySQLPad.View
{
    /// <summary>
    /// A content for TabControl.
    ///     QueryName is usually a uppercase name of SQL command.
    ///     Data is DataTable of records.
    /// </summary>
    public class TabContent : ImplementsPropertyChanged
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

    }
}