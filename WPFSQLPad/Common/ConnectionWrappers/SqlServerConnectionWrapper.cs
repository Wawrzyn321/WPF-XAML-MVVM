using System;
using System.Collections.Generic;
using Common.TreeItems;
using Model;
using Model.ConnectionModels;
using WPFSQLPad.ConnectionWrappers.DataExtractors;

namespace WPFSQLPad.ConnectionWrappers
{
    public sealed class SqlServerConnectionWrapper : DatabaseConnectionWrapper
    {
        public SqlServerConnectionWrapper(DatabaseConnection connectionReference) : base(connectionReference)
        {
            dataExtrator = new SqlServerDatabaseDataExtrator(connectionReference);
        }

        public override DatabaseBranch GetDatabaseDescription()
        {
            if (!ConnectionReference.CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            var allTables = ConnectionReference.PerformSelect("SELECT * FROM INFORMATION_SCHEMA.TABLES");
            dataExtrator.GetTablesAndRoutines(allTables, out var tables, out var views);
            List<Routine> routines = dataExtrator.GetRoutines();

            return new DatabaseBranch($"{ConnectionReference.Server}: {ConnectionReference.Database}", tables, views, routines, this);
        }

    }
}
