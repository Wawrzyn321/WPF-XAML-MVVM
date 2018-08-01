using Model.ConnectionModels;
using WPFSQLPad.ConnectionWrappers;

namespace WPFSQLPad.TreeItems
{
    public abstract class TreeItem
    {
        public DatabaseConnectionWrapper ConnectionReference { get; protected set; }

        protected TreeItem(DatabaseConnectionWrapper connectionReference)
        {
            ConnectionReference = connectionReference;
        }
    }
}