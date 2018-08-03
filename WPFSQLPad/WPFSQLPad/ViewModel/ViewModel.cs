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

        public ObservableCollection<TabContent> Tabs => quertyTabViewModel.Tabs;

        public TabContent SelectedTab
        {
            get => quertyTabViewModel.SelectedTab;
            set => quertyTabViewModel.SelectedTab = value;
        }

        private bool clearPreviousResults;
        public bool ClearPreviousResults
        {
            get => clearPreviousResults;
            set => Set(ref clearPreviousResults, value);
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
        private readonly IQuertyTabViewModel quertyTabViewModel;
        private readonly ConnectionContainer connectionContainer;

        private LoggerView loggerView;
        public LoggerView LoggerView
        {
            get => loggerView;
            set => Set(ref loggerView, value);
        }

        private IQueryTabView queryTabView;
        public IQueryTabView QueryTabView
        {
            get => queryTabView;
            set => Set(ref queryTabView, value);
        }

        public ViewModel(IView view)
        {
            this.view = view;

            LoggerView = new LoggerView();
            QueryTabView = new QueryTabView(LoggerView.Logger);

            quertyTabViewModel = QueryTabView.ViewModel;

            connectionContainer = new ConnectionContainer(LoggerView.Logger, quertyTabViewModel);
            AssignViewEvents();
            InitializeCommands();
        }

        //subscribe to view events
        private void AssignViewEvents()
        {
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
                if (ClearPreviousResults)
                {
                    quertyTabViewModel.CloseAllTabs();
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
