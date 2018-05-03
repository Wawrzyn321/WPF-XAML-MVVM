using System;
using System.Windows;
using System.Windows.Input;
using Model;
using MySQLPad.View;

namespace MySQLPad
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        public object SelectedTreeItem => DatabasesTree.SelectedItem;

        public event Action<TabContent> OnCloseTabRequested;
        public event Action<TabContent> OnExportTabXMLRequested;
        public event Action<TabContent> OnExportTabCSVRequested;
        public event Action<DatabaseConnection> OnDatabaseChoiceRequested;
        public event Action OnCloseAllTabsRequested;

        private readonly ViewModel.ViewModel viewModel;

        public ICommand ChooseDatabaseCommand { get; private set; }
        public ICommand CloseTabCommand { get; private set; }
        public ICommand ExportTabXMLCommand { get; private set; }
        public ICommand ExportTabCSVCommand { get; private set; }
        public ICommand CloseAllTabsCommand { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new ViewModel.ViewModel(this);
            DataContext = viewModel;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ChooseDatabaseCommand = new ActionCommand<DatabaseConnection>(content => OnDatabaseChoiceRequested?.Invoke(content));
            CloseTabCommand = new ActionCommand<TabContent>(content => OnCloseTabRequested?.Invoke(content));
            ExportTabXMLCommand = new ActionCommand<TabContent>(content => OnExportTabXMLRequested?.Invoke(content));
            ExportTabCSVCommand = new ActionCommand<TabContent>(content => OnExportTabCSVRequested?.Invoke(content));
            CloseAllTabsCommand = new ActionCommand(() => OnCloseAllTabsRequested?.Invoke());
        }
    }


}
