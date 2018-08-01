using System;
using System.Collections.Generic;
using Model;
using Model.ConnectionModels;
using WPFSQLPad.TreeItems;

namespace WPFSQLPad.ConnectionWrappers
{
    public class SqlServerConnectionWrapper : DatabaseConnectionWrapper
    {
        public SqlServerConnectionWrapper(DatabaseConnection connectionReference) : base(connectionReference)
        {
        }

        public override DatabaseBranch GetDatabaseDescription()
        {
            if (!connectionReference.CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            var allTables = connectionReference.PerformSelect("SELECT * FROM INFORMATION_SCHEMA.TABLES");
            GetTablesAndRoutines(allTables, out var tables, out var views);
            List<Routine> routines = GetRoutines();

            return new DatabaseBranch($"{connectionReference.Server}: {connectionReference.Database}", tables, views, routines, this);
        }

        private void GetTablesAndRoutines(ResultContainer allTables, out List<TableBranch> tables, out List<TableBranch> views)
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
                    views.Add(new TableBranch(tableNames[i], columns, this));
                }
                else
                {
                    tables.Add(new TableBranch(tableNames[i], columns, this));
                }
            }
        }

        public override List<Routine> GetRoutines()
        {
            if (!connectionReference.CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            var data = connectionReference.PerformSelect($"select ROUTINE_NAME, ROUTINE_TYPE from {connectionReference.Database}.information_schema.routines").Data;

            return ExtractRoutines(data);
        }

        public override string GetRoutineCode(Routine.RoutineType type, string name)
        {
            if (!connectionReference.CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            string typeUppercase = type.ToString().ToUpper();
            var container = connectionReference.PerformSelect($"select ROUTINE_DEFINITION from {connectionReference.Database}.information_schema.routines WHERE ROUTINE_TYPE='{typeUppercase}' AND ROUTINE_NAME='{name}'");

            return ExtractRoutineCode(container);
        }

        public override List<ColumnDescription> GetTableDescription(string tableName)
        {
            if (!connectionReference.CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            var container = connectionReference.PerformSelect($"SP_COLUMNS '{tableName}'");
            return ExtractTableDescription(container);
        }

        private List<Routine> ExtractRoutines(List<string[]> data)
        {
            List<Routine> routines = new List<Routine>();
            foreach (var routineData in data)
            {
                string name = routineData[0];
                string type = routineData[1];

                List<string[]> parametersData =
                    connectionReference.PerformSelect(
                        $"select name, TYPE_NAME(system_type_id)  from sys.parameters where object_id = object_id('{name}')").Data;

                string[] parameters = new string[parametersData.Count];
                for (int i = 0; i < parametersData.Count; i++)
                {
                    parameters[i] = $"{parametersData[i][1]} {parametersData[i][0]}";
                }

                routines.Add(new Routine(name, type, string.Join(", ", parameters), string.Empty, this));
            }

            return routines;
        }

        private List<ColumnDescription> ExtractTableDescription(ResultContainer container)
        {
            List<ColumnDescription> result = new List<ColumnDescription>();

            foreach (string[] s in container.Data)
            {
                string Name = s[3];
                string Type = s[5];
                bool CanBeNull = s[10].Equals("0");
                result.Add(new ColumnDescription(Name, Type, CanBeNull, this));
            }
            return result;
        }

        private string ExtractRoutineCode(ResultContainer container)
        {
            return container.FirstResult;
        }
    }
}
