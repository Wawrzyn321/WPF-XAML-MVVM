using System.Collections.Generic;
using Model.ConnectionModels;

namespace WPFSQLPad.TreeItems
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
        public List<TableBranch> Items { get; }

        public TableHeader(string headerName, List<TableBranch> items, DatabaseConnection connection) 
            : base(headerName, connection)
        {
            Items = items;
        }

    }

    public class RoutineHeader : HeaderBranch
    {
        public List<Routine> Routines { get; }

        public RoutineHeader(string headerName, List<Routine> routines, DatabaseConnection connection)
            : base(headerName, connection)
        {
            Routines = routines;
        }
    }
}
