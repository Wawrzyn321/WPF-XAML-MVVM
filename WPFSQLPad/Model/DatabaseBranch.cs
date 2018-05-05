using System;
using System.Collections.ObjectModel;

namespace Model
{
    /// <summary>
    /// Database branch for TreeView
    /// </summary>
    public class DatabaseBranch : ImplementsPropertyChanged, IEquatable<DatabaseBranch>
    {
        private string databaseName;
        public string DatabaseName
        {
            get => databaseName;
            set => Set(ref databaseName, value);
        }

        public HeaderBranch Tables { get; }
        public HeaderBranch Views { get; }
        public DatabaseConnection ConnectionReference { get; }
        public ObservableCollection<HeaderBranch> TablesAndViews { get; } //tables and views are separated here

        public DatabaseBranch(string databaseName, ObservableCollection<TableBranch> tables, ObservableCollection<TableBranch> views, DatabaseConnection connection)
        {
            Tables = new HeaderBranch("Tables", tables);
            Views = new HeaderBranch("Views", views);
            DatabaseName = databaseName;
            ConnectionReference = connection;

            TablesAndViews = new ObservableCollection<HeaderBranch>
            {
                Tables,
                Views,
            };
        }

        public bool Equals(DatabaseBranch other)
        {
            if (other == null)
            {
                return false;
            }
            return Equals(databaseName, other.DatabaseName);
        }
    }
}
