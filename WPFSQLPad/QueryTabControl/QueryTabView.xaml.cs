using System;
using System.Windows.Controls;
using System.Windows.Input;
using Logger;
using QueryTabControl.Interface;

namespace QueryTabControl
{
    /// <summary>
    /// Interaction logic for QueryTabControl.xaml
    /// </summary>
    public partial class QueryTabView : UserControl, IQueryTabView
    {
        public ICommand CloseTabCommand { get; private set; }
        public ICommand ExportTabXMLCommand { get; private set; }
        public ICommand ExportTabCSVCommand { get; private set; }
        public ICommand CloseAllTabsCommand { get; private set; }

        public event Action<TabContent> OnCloseTabRequested;
        public event Action<TabContent> OnExportTabXMLRequested;
        public event Action<TabContent> OnExportTabCSVRequested;
        public event Action OnCloseAllTabsRequested;

        public IQuertyTabViewModel ViewModel { get; }

        public QueryTabView(LoggerViewModel logger)
        {
            InitializeComponent();
            ViewModel = new QuertyTabViewModel(logger, this);
            DataContext = ViewModel;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            CloseTabCommand = new ActionCommand<TabContent>(content => OnCloseTabRequested?.Invoke(content));
            ExportTabXMLCommand = new ActionCommand<TabContent>(content => OnExportTabXMLRequested?.Invoke(content));
            ExportTabCSVCommand = new ActionCommand<TabContent>(content => OnExportTabCSVRequested?.Invoke(content));
            CloseAllTabsCommand = new ActionCommand(() => OnCloseAllTabsRequested?.Invoke());
        }

    }
}
