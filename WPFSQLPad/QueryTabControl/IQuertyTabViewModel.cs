using System.Collections.ObjectModel;
using System.ComponentModel;

namespace QueryTabControl
{
    public interface IQuertyTabViewModel : INotifyPropertyChanged
    {
        TabContent SelectedTab { get; set; }
        ObservableCollection<TabContent> Tabs { get; set; }

        void Add(TabContent tabContent);
        void CloseAllTabs();
        void CloseTab(TabContent tab);
        void ExportTabAsCsv(TabContent tabContent);
        void ExportTabAsXml(TabContent tabContent);
    }
}