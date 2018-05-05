using System;
using Model;

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
        event Action<DatabaseConnection> OnDatabaseChoiceRequested;

        //close results tabs
        event Action OnCloseAllTabsRequested;
    }
}
