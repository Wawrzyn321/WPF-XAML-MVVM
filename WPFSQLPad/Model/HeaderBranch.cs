using System.Collections.ObjectModel;

namespace Model
{
    /// <summary>
    /// Header for TreeView, used to branch
    /// actual tables, views and routines.
    /// </summary>
    public abstract class HeaderBranch : TreeItem
    {
        public string HeaderName { get; }

        protected HeaderBranch(string headerName, DatabaseConnection connection) : base(connection)
        {
            HeaderName = headerName;
        }

    }

    public class TableHeader : HeaderBranch
    {
        public ObservableCollection<TableBranch> Items { get; }

        public TableHeader(string headerName, ObservableCollection<TableBranch> items, DatabaseConnection connection) 
            : base(headerName, connection)
        {
            Items = items;
        }

    }

    public class RoutineHeader : HeaderBranch
    {
        public ObservableCollection<Routine> Routines { get; }

        public RoutineHeader(string headerName, ObservableCollection<Routine> routines, DatabaseConnection connection)
            : base(headerName, connection)
        {
            Routines = routines;
        }
    }
}
