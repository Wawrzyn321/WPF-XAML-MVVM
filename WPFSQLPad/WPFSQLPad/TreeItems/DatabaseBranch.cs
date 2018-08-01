using System;
using System.Collections.Generic;
using Model.ConnectionModels;
using WPFSQLPad.ConnectionWrappers;

namespace WPFSQLPad.TreeItems
{
    /// <summary>
    /// Database branch for TreeView
    /// </summary>
    public class DatabaseBranch : IEquatable<DatabaseBranch>
    {
        public string DatabaseName { get; }
        public TableHeader Tables { get; }
        public TableHeader Views { get; }
        public RoutineHeader Routines { get; }
        public List<HeaderBranch> AllChildren { get; } //tables and views are separated here

        public DatabaseConnectionWrapper ConnectionReference { get; }

        public DatabaseBranch(string databaseName, 
            List<TableBranch> tables,
            List<TableBranch> views, 
            List<Routine> routines, 
            DatabaseConnectionWrapper connection)
        {
            DatabaseName = databaseName;
            Tables = new TableHeader("Tables", tables, connection);
            Views = new TableHeader("Views", views, connection);
            Routines = new RoutineHeader("Routines", routines, connection);
            ConnectionReference = connection;

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
