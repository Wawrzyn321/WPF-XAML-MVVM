using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model
{
    /// <summary>
    /// Branch model for TreeView with name
    /// and collection of ColumnDescription.
    /// </summary>
    public class TableBranch : ImplementsPropertyChanged
    {
        private string tableName;
        public string TableName
        {
            get => tableName;
            set => Set(ref tableName, value);
        } 
        public ObservableCollection<ColumnDescription> Columns { get; }

        public TableBranch(string tableName, IEnumerable<ColumnDescription> columns)
        {
            Columns = new ObservableCollection<ColumnDescription>(columns);
            TableName = tableName;
        }

        public TableBranch(string tableName, ObservableCollection<ColumnDescription> columns)
        {
            Columns = columns;
            TableName = tableName;
        }
    }
}