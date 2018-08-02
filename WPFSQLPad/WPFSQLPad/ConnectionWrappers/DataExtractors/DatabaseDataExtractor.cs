using System.Collections.Generic;
using Model;
using Model.ConnectionModels;
using WPFSQLPad.TreeItems;

namespace WPFSQLPad.ConnectionWrappers.DataExtractors
{
    public abstract class DatabaseDataExtractor
    {
        protected readonly DatabaseConnection connection;

        protected DatabaseDataExtractor(DatabaseConnection connection)
        {
            this.connection = connection;
        }

        public abstract void GetTablesAndRoutines(ResultContainer allTables, out List<TableBranch> tables,
            out List<TableBranch> views);
        public abstract List<Routine> GetRoutines();
        public abstract List<ColumnDescription> GetTableDescription(string tableName);
    }
}
