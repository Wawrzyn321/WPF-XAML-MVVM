using System.Collections.ObjectModel;

namespace Model
{
    /// <summary>
    /// Header for TreeView, used to branch
    /// actual tables and views.
    /// </summary>
    public class HeaderBranch
    {
        public string HeaderName { get; }
        public ObservableCollection<TableBranch> Tables { get; }

        public HeaderBranch(string headerName, ObservableCollection<TableBranch> tables)
        {
            HeaderName = headerName;
            Tables = tables;
        }

    }
}
