using System;
using System.Collections.ObjectModel;

namespace Model.TreeItems
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
        public ObservableCollection<HeaderBranch> AllChildren { get; } //tables and views are separated here

        public DatabaseConnection ConnectionReference { get; }

        public DatabaseBranch(string databaseName, 
            ObservableCollection<TableBranch> tables,
            ObservableCollection<TableBranch> views, 
            ObservableCollection<Routine> routines, 
            DatabaseConnection connection)
        {
            DatabaseName = databaseName;
            Tables = new TableHeader("Tables", tables, connection);
            Views = new TableHeader("Views", views, connection);
            Routines = new RoutineHeader("Routines", routines, connection);
            ConnectionReference = connection;

            AllChildren = new ObservableCollection<HeaderBranch>
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
