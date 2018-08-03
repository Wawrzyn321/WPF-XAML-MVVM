using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using DatabaseConnectionDialog.View;
using Model;
using QueryTabControl;
using Logger;
using WPFSQLPad.ConnectionWrappers;
using WPFSQLPad.IMenuItems;
using WPFSQLPad.TreeItems;
using IView = WPFSQLPad.View.IView;

namespace WPFSQLPad.ViewModel
{

    /// <summary>
    /// ViewModel for main application
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        #region Observed properties

        public ObservableCollection<TabContent> Tabs => tabController.Tabs;

        public TabContent SelectedTab
        {
            get => tabController.SelectedTab;
            set => tabController.SelectedTab = value;
        }

        public bool ClearPreviousResults
        {
            get => tabController.ClearPreviousResults;
            set => tabController.ClearPreviousResults = value;
        }

        private string queryText;
        public string QueryText
        {
            get => queryText;
            set => Set(ref queryText, value);
        }

        private string log;
        public string Log
        {
            get => log;
            set => Set(ref log, value);
        }

        public ObservableCollection<IMenuItem> Connections => connectionContainer.Connections;
        public DatabaseConnectionWrapper CurrentConnection => connectionContainer.CurrentConnection;
        public ObservableCollection<DatabaseBranch> DatabasesTree => connectionContainer.DatabasesTree;

        public bool StopOnError
        {
            get => connectionContainer.StopOnError;
            set => connectionContainer.StopOnError = value;
        } 
        
        public bool IsQuerying
        {
            get => connectionContainer.IsQuerying;
            set => connectionContainer.IsQuerying = value;
        }

        #endregion

        #region Commands

        public ICommand ExecuteSqlCommand { get; private set; }
        public ICommand AddConnectionCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand CloseConnectionCommand { get; private set; }
        public ICommand StopExecutingCommand { get; private set; }
        public ICommand CloseAllConnectionsCommand { get; private set; }

        #endregion

        private readonly IView view;
        private readonly TabController tabController;
        private readonly ConnectionContainer connectionContainer;

        private LoggerView loggerView;
        public LoggerView LoggerView
        {
            get => loggerView;
            set => Set(ref loggerView, value);
        }

        private QueryTabView queryTabView;
        public QueryTabView QueryTabView
        {
            get => queryTabView;
            set => Set(ref queryTabView, value);
        }

        public ViewModel(IView view)
        {
            this.view = view;

            LoggerView = new LoggerView();
            QueryTabView = new QueryTabView(LoggerView.Logger);

            tabController = QueryTabView.TabController;

            connectionContainer = new ConnectionContainer(LoggerView.Logger, tabController);
            AssignViewEvents();
            InitializeCommands();
        }

        //subscribe to view events
        private void AssignViewEvents()
        {
            view.OnCloseTabRequested += tabController.CloseTab;
            view.OnCloseAllTabsRequested += tabController.CloseAllTabs;
            view.OnExportTabXMLRequested += tabController.ExportTabAsXml;
            view.OnExportTabCSVRequested += tabController.ExportTabAsCsv;

            view.OnDatabaseChoiceRequested += connectionContainer.ChooseDatabase;
            view.OnDatabaseRefreshRequested += connectionContainer.RefreshDatabase;
            view.OnSetDatabaseAsCurrentRequested += connectionContainer.SetConnectionAsCurrent;
            view.OnDatabaseCloseRequested += connectionContainer.CloseDatabaseConnection;
            view.OnCloseAllConnectionsRequested += connectionContainer.CloseAllConnections;
            view.OnRoutineSourceRequested += connectionContainer.CopyRoutineSource;
        }
        
        //initialize ICommands
        private void InitializeCommands()
        {
            ExecuteSqlCommand = new ActionCommand(ExecuteQuery_OnClick, () => !IsQuerying);
            AddConnectionCommand = new ActionCommand(AddConnection_OnClick);
            CloseConnectionCommand = new ActionCommand(RemoveConnection_OnClick);

            StopExecutingCommand = new ActionCommand(connectionContainer.StopExecuting);
            CloseAllConnectionsCommand = new ActionCommand(connectionContainer.CloseAllConnections);

            CloseCommand = new ActionCommand(() => Environment.Exit(0));
        }

        #region Command Callbacks

        //execute command
        private void ExecuteQuery_OnClick()
        {
            if (CurrentConnection == null)
            {
                MessageBox.Show("No database connection specified!", "SQL Pad");
                return;
            }

            //get separate queries
            var queries = SQLHelper.SplitSqlExpression(queryText, CurrentConnection.Delimiter);

            if (queries.Count == 0)
            {
                LoggerView.Logger.Write("Ain't no queries here.\n");
            }
            else
            {
                if (tabController.ClearPreviousResults)
                {
                    tabController.CloseAllTabs();
                }
                connectionContainer.ExecuteQueries(queries);
            }
        }

        //#pure
        //add new DB connection using DBCollectionDialog
        private void AddConnection_OnClick()
        {
            var dialog = new DbConnectionDialog();

            if (dialog.ShowDialog() == true)
            {
                connectionContainer.AddDatabaseConnection(dialog.Connection, dialog.SetAsCurrent, dialog.DatabaseType);
            }
        }
        
        //#pure
        //remove connection using "Close connection" button
        private void RemoveConnection_OnClick()
        {
            //check if a DatabaseBranch is actually selected
            if (!(view.SelectedTreeItem is DatabaseBranch branch)) return;

            var d = MessageBox.Show($"Remove {branch.DatabaseName} connection?", "SQL Pad", MessageBoxButton.YesNo);
            if (d == MessageBoxResult.Yes)
            {
                connectionContainer.RemoveConnection(branch);
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
