using System.Collections.Generic;

namespace Common.TreeItems
{
    /// <summary>
    /// Branch model for TreeView with name
    /// Used for Tables and views.
    /// </summary>
    public sealed class TableBranch
    {
        public string TableName { get; }

        public List<ColumnDescription> Columns { get; }

        public TableBranch(string tableName, IEnumerable<ColumnDescription> columns)
        {
            Columns = new List<ColumnDescription>(columns);
            TableName = tableName;
        }

        public TableBranch(string tableName, List<ColumnDescription> columns)
        {
            Columns = columns;
            TableName = tableName;
        }
    }
}