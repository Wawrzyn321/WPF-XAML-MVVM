using System.Collections.Generic;
using Common.TreeItems;
using Model;
using Model.ConnectionModels;

namespace WPFSQLPad.ConnectionWrappers.DataExtractors
{
    public class MySqlDatabaseDataExtractor : DatabaseDataExtractor
    {
        public MySqlDatabaseDataExtractor(DatabaseConnection connection) : base(connection)
        {
        }

        public override void GetTablesAndRoutines(ResultContainer container, out List<TableBranch> tables, out List<TableBranch> views)
        {
            tables = new List<TableBranch>();
            views = new List<TableBranch>();
            foreach (var data in container.Data)
            {
                string tableName = data[0];
                List<ColumnDescription> columns = GetTableDescription(tableName);

                //add to separate tables, depending on type
                bool isView = data[1] == DatabaseConnection.tableType_View;
                if (isView)
                {
                    views.Add(new TableBranch(tableName, columns));
                }
                else
                {
                    tables.Add(new TableBranch(tableName, columns));
                }
            }

        }

        public override List<Routine> GetRoutines()
        {
            var container = connection.PerformSelect("SHOW FUNCTION STATUS");
            var routines = new List<Routine>(container.Data.Count);

            foreach (var data in container.Data)
            {
                string name = data[1];
                string type = data[2];
                string[] query = connection.PerformSelect($"SELECT param_list, returns FROM mysql.proc WHERE db='{connection.Database}' AND name='{name}'").FirstRow;
                string parameters = query[0];
                string returnType = query[1];

                var containerWithCode = connection.PerformSelect($"SHOW CREATE {type} {name}");
                string code = containerWithCode.Data[0][2];
                routines.Add(new Routine(name, type, parameters, returnType, code));
            }

            return routines;
        }

        public override List<ColumnDescription> GetTableDescription(string tableName)
        {
            var container = connection.PerformSelect($"DESC {tableName}");

            List<ColumnDescription> result = new List<ColumnDescription>();
            foreach (var s in container.Data)
            {
                string Name = s[0];
                string Type = s[1];
                bool CanBeNull = s[2].Equals(ColumnDescription.CanBeNull_Yes);
                string Key = s[3];
                string Extra = s[4];
                string Default = s[5];

                result.Add(new ColumnDescription(Name, Type, CanBeNull, Key, Default, Extra));
            }

            return result;
        }

    }
}
