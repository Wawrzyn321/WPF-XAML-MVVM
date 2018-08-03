using System;

namespace QueryTabControl
{
    public interface IQueryTabView
    {
        IQuertyTabViewModel ViewModel { get; }

        //close results tab
        event Action<TabContent> OnCloseTabRequested;

        //export as XML file
        event Action<TabContent> OnExportTabXMLRequested;

        //export as CSV file
        event Action<TabContent> OnExportTabCSVRequested;

        //close results tabs
        event Action OnCloseAllTabsRequested;

    }
}