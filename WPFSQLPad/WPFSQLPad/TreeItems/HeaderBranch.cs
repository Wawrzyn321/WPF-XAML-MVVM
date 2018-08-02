using System.Collections.Generic;

namespace WPFSQLPad.TreeItems
{
    /// <summary>
    /// Header for TreeView, used to branch
    /// actual tables, views and routines.
    /// </summary>
    public abstract class HeaderBranch
    {
        public string HeaderName { get; }

        protected HeaderBranch(string headerName)
        {
            HeaderName = headerName;
        }

    }

    public sealed class TableHeader : HeaderBranch
    {
        public List<TableBranch> Items { get; }

        public TableHeader(string headerName, List<TableBranch> items) 
            : base(headerName)
        {
            Items = items;
        }

    }

    public sealed class RoutineHeader : HeaderBranch
    {
        public List<Routine> Routines { get; }

        public RoutineHeader(string headerName, List<Routine> routines)
            : base(headerName)
        {
            Routines = routines;
        }
    }
}
