using System;
using System.Collections.Generic;
using Common.TreeItems;
using WPFSQLPad.ConnectionWrappers;

namespace WPFSQLPad.TreeItems
{
    /// <summary>
    /// Database branch for TreeView
    /// </summary>
    public sealed class DatabaseBranch : IEquatable<DatabaseBranch>
    {
        public string DatabaseName { get; }
        public TableHeader Tables { get; }
        public TableHeader Views { get; }
        public RoutineHeader Routines { get; }
        public List<HeaderBranch> AllChildren { get; } //tables and views are separated here

        public DatabaseConnectionWrapper Wrapper { get; }

        public DatabaseBranch(string databaseName, 
            List<TableBranch> tables,
            List<TableBranch> views, 
            List<Routine> routines, 
            DatabaseConnectionWrapper connection)
        {
            DatabaseName = databaseName;
            Tables = new TableHeader("Tables", tables);
            Views = new TableHeader("Views", views);
            Routines = new RoutineHeader("Routines", routines);
            Wrapper = connection;

            AllChildren = new List<HeaderBranch>
            {
                Tables,
                Views,
                Routines,
            };
        }

        public bool Equals(DatabaseBranch other)
        {
            if (other == null)
            {
                return false;
            }
            return Equals(DatabaseName, other.DatabaseName);
        }
    }
}
