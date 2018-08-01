using System;
using System.Collections.Generic;
using Model;
using Model.ConnectionModels;
using WPFSQLPad.TreeItems;

namespace WPFSQLPad.ConnectionWrappers
{
    public sealed class MySqlConnectionWrapper : DatabaseConnectionWrapper
    {
        public MySqlConnectionWrapper(DatabaseConnection connectionReference) : base(connectionReference)
        {
        }

        public override DatabaseBranch GetDatabaseDescription()
        {
            if (!connectionReference.CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }
            ResultContainer container = connectionReference.PerformSelect("SHOW FULL TABLES");
            GetTablesAndRoutines(container, out var tables, out var views);
            List<Routine> routines = GetRoutines();

            return new DatabaseBranch($"{connectionReference.Server}: {connectionReference.Database}", tables, views, routines, this);

        }

        private void GetTablesAndRoutines(ResultContainer container, out List<TableBranch> tables, out List<TableBranch> views)
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
                    views.Add(new TableBranch(tableName, columns, this));
                }
                else
                {
                    tables.Add(new TableBranch(tableName, columns, this));
                }
            }

        }

        public override List<Routine> GetRoutines()
        {
            if (!connectionReference.CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            ResultContainer s = connectionReference.PerformSelect("SHOW FUNCTION STATUS");
            return ExtractRoutines(s);
        }

        public override string GetRoutineCode(Routine.RoutineType type, string name)
        {
            if (!connectionReference.CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            return ExtractRoutineCode(connectionReference.PerformSelect($"SHOW CREATE {type} {name}"));
        }

        public override List<ColumnDescription> GetTableDescription(string tableName)
        {
            if (!connectionReference.CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            var data = connectionReference.PerformSelect($"DESC {tableName}");

            return ExtractTableDescription(data);
        }

        private List<ColumnDescription> ExtractTableDescription(ResultContainer container)
        {
            List<ColumnDescription> result = new List<ColumnDescription>();
            foreach (var s in container.Data)
            {
                string Name = s[0];
                string Type = s[1];
                bool CanBeNull = s[2].Equals(ColumnDescription.CanBeNull_Yes);
                string Key = s[3];
                string Extra = s[4];
                string Default = s[5];

                result.Add(new ColumnDescription(Name, Type, CanBeNull, Key, Default, Extra, this));
            }

            return result;
        }

        private string ExtractRoutineCode(ResultContainer container)
        {
            return container.Data[0][2];
        }

        private List<Routine> ExtractRoutines(ResultContainer container)
        {
            List<Routine> routines = new List<Routine>(container.Data.Count);

            foreach (string[] data in container.Data)
            {
                string name = data[1];
                string type = data[2];
                string[] query =
                    connectionReference.PerformSelect($"SELECT param_list, returns FROM mysql.proc WHERE db = '{connectionReference.Database}' AND name = '{name}'").Data[0];
                string parameters = query[0];
                string returnType = query[1];
                routines.Add(new Routine(name, type, parameters, returnType, this));
            }

            return routines;
        }
        
    }
}
