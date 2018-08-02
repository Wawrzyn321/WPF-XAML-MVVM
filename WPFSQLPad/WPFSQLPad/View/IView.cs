using System;
using WPFSQLPad.ConnectionWrappers;
using WPFSQLPad.TreeItems;

namespace WPFSQLPad.View
{
    /// <summary>
    /// View contract for Pad.
    /// </summary>
    public interface IView
    {
        //item in TreeView, used for selecting DBs to remove
        object SelectedTreeItem { get; }

        //close results tab
        event Action<TabContent> OnCloseTabRequested;

        //export as XML file
        event Action<TabContent> OnExportTabXMLRequested;

        //export as CSV file
        event Action<TabContent> OnExportTabCSVRequested;

        //change current DB
        event Action<DatabaseConnectionWrapper> OnDatabaseChoiceRequested;

        //close results tabs
        event Action OnCloseAllTabsRequested;

        //refresh database connection
        event Action<DatabaseBranch> OnDatabaseRefreshRequested;

        //set branch's connection as current
        event Action<DatabaseBranch> OnSetDatabaseAsCurrentRequested;

        //close database connection
        event Action<DatabaseBranch> OnDatabaseCloseRequested;

        //get routine source code
        event Action<Routine> OnRoutineSourceRequested;

        //gracefully finalize all connections
        event Action OnCloseAllConnectionsRequested;
    }
}
