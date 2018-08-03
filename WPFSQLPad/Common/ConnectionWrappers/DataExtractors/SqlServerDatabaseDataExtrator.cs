using System.Collections.Generic;
using Common.TreeItems;
using Model;
using Model.ConnectionModels;

namespace WPFSQLPad.ConnectionWrappers.DataExtractors
{
    public sealed class SqlServerDatabaseDataExtrator : DatabaseDataExtractor
    {
        public SqlServerDatabaseDataExtrator(DatabaseConnection connection) : base(connection)
        {

        }
        
        public override void GetTablesAndRoutines(ResultContainer allTables, out List<TableBranch> tables, out List<TableBranch> views)
        {
            var tableNames = new string[allTables.Data.Count];
            var tableTypes = new string[allTables.Data.Count];
            for (int i = 0; i < allTables.Data.Count; i++)
            {
                tableNames[i] = allTables.Data[i][2];
                tableTypes[i] = allTables.Data[i][3];
            }

            tables = new List<TableBranch>();
            views = new List<TableBranch>();
            for (int i = 0; i < tableNames.Length; i++)
            {
                List<ColumnDescription> columns = GetTableDescription(tableNames[i]);
                bool isView = tableTypes[i] == DatabaseConnection.tableType_View;
                if (isView)
                {
                    views.Add(new TableBranch(tableNames[i], columns));
                }
                else
                {
                    tables.Add(new TableBranch(tableNames[i], columns));
                }
            }
        }

        public override List<Routine> GetRoutines()
        {
            var container = connection.PerformSelect($"select ROUTINE_NAME, ROUTINE_TYPE from {connection.Database}.information_schema.routines");
            List<Routine> routines = new List<Routine>();
            foreach (var routineData in container.Data)
            {
                string name = routineData[0];
                string type = routineData[1];

                List<string[]> parametersData =
                    connection.PerformSelect(
                        $"select name, TYPE_NAME(system_type_id)  from sys.parameters where object_id = object_id('{name}')").Data;

                string[] parameters = new string[parametersData.Count];
                for (int i = 0; i < parametersData.Count; i++)
                {
                    parameters[i] = $"{parametersData[i][1]} {parametersData[i][0]}";
                }

                string typeUppercase = type.ToString().ToUpper();
                var containerWithCode = connection.PerformSelect($"select ROUTINE_DEFINITION from {connection.Database}.information_schema.routines WHERE ROUTINE_TYPE='{typeUppercase}' AND ROUTINE_NAME='{name}'");
                string code = containerWithCode.FirstResult;

                routines.Add(new Routine(name, type, string.Join(", ", parameters), string.Empty, code));
            }

            return routines;
        }

        public override List<ColumnDescription> GetTableDescription(string tableName)
        {
            var container = connection.PerformSelect($"SP_COLUMNS '{tableName}'");
            List<ColumnDescription> columnDescriptions = new List<ColumnDescription>();

            foreach (string[] s in container.Data)
            {
                string Name = s[3];
                string Type = s[5];
                bool CanBeNull = s[10].Equals("0");
                columnDescriptions.Add(new ColumnDescription(Name, Type, CanBeNull));
            }
            return columnDescriptions;
        }
    }
}

