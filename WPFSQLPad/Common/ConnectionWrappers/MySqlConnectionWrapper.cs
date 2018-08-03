using System;
using System.Collections.Generic;
using Common.TreeItems;
using Model;
using Model.ConnectionModels;
using WPFSQLPad.ConnectionWrappers.DataExtractors;

namespace WPFSQLPad.ConnectionWrappers
{
    public sealed class MySqlConnectionWrapper : DatabaseConnectionWrapper
    {
        public MySqlConnectionWrapper(DatabaseConnection connectionReference) : base(connectionReference)
        {
            dataExtrator = new MySqlDatabaseDataExtractor(connectionReference);
        }

        public override DatabaseBranch GetDatabaseDescription()
        {
            if (!ConnectionReference.CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            ResultContainer container = ConnectionReference.PerformSelect("SHOW FULL TABLES");
            dataExtrator.GetTablesAndRoutines(container, out var tables, out var views);
            List<Routine> routines = dataExtrator.GetRoutines();

            return new DatabaseBranch($"{ConnectionReference.Server}: {ConnectionReference.Database}", tables, views, routines, this);

        }
        
    }
}
