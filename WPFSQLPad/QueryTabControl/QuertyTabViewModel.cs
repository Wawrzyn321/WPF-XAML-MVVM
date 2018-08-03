using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Logger;

namespace QueryTabControl
{
    public class QuertyTabViewModel : IQuertyTabViewModel
    {
        #region Observed Properties

        private ObservableCollection<TabContent> tabs;
        public ObservableCollection<TabContent> Tabs
        {
            get => tabs;
            set => Set(ref tabs, value);
        }

        private TabContent selectedTab;
        public TabContent SelectedTab
        {
            get => selectedTab;
            set => Set(ref selectedTab, value);
        }

        #endregion

        private readonly LoggerViewModel logger;

        public QuertyTabViewModel(LoggerViewModel logger, IQueryTabView view)
        {
            this.logger = logger;
            Tabs = new ObservableCollection<TabContent>();

            AssignEventsToView(view);
        }

        private void AssignEventsToView(IQueryTabView view)
        {
            view.OnCloseTabRequested += CloseTab;
            view.OnCloseAllTabsRequested += CloseAllTabs;
            view.OnExportTabXMLRequested += ExportTabAsXml;
            view.OnExportTabCSVRequested += ExportTabAsCsv;
        }

        public void CloseTab(TabContent tab)
        {
            Tabs.Remove(tab);
        }

        public void CloseAllTabs()
        {
            Tabs.Clear();
            logger.WriteLine("Closed all tabs.");
        }

        public void Add(TabContent tabContent)
        {
            Tabs.Add(tabContent);
        }

        #region Data Export

        public void ExportTabAsXml(TabContent tabContent)
        {
            if (DataTableSerializer.SerializeAsXML(tabContent.Data))
            {
                logger.WriteLine("\nSaved the table as XML.");
            }
            else
            {
                logger.WriteLine("\nCould not save XML file.");
            }
        }

        public void ExportTabAsCsv(TabContent tabContent)
        {
            if (DataTableSerializer.SerializeAsCSV(tabContent.Data))
            {
                logger.WriteLine("\nSaved the table as CSV.");
            }
            else
            {
                logger.WriteLine("\nCould not save CSV file.");
            }
        } 

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T oldValue, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(oldValue, value))
            {
                return false;
            }
            else
            {
                oldValue = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        #endregion
    }
}
