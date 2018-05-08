using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model.TreeItems
{
    /// <summary>
    /// Branch model for TreeView with name
    /// Used for Tables and views.
    /// </summary>
    public class TableBranch : TreeItem
    {
        public string TableName { get; }
        public ObservableCollection<ColumnDescription> Columns { get; }

        public TableBranch(string tableName, IEnumerable<ColumnDescription> columns, DatabaseConnection connection)
        : base(connection)
        {
            Columns = new ObservableCollection<ColumnDescription>(columns);
            TableName = tableName;
        }

        public TableBranch(string tableName, ObservableCollection<ColumnDescription> columns, DatabaseConnection connection)
        : base(connection)
        {
            Columns = columns;
            TableName = tableName;
        }
    }
}