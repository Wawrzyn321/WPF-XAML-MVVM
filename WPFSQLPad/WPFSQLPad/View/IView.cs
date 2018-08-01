using System;
using Model.ConnectionModels;
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

        //close database connection
        event Action<DatabaseBranch> OnDatabaseCloseRequested;

        //get routine source code
        event Action<Routine> OnRoutineSourceRequested;
    }
}
