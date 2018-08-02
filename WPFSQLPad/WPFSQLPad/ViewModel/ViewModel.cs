﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Model;
using Model.ConnectionModels;
using WPFSQLPad.ConnectionWrappers;
using WPFSQLPad.IMenuItems;
using WPFSQLPad.TreeItems;
using WPFSQLPad.View;

namespace WPFSQLPad.ViewModel
{

    /// <summary>
    /// ViewModel for main application
    /// </summary>
    public class ViewModel : INotifyPropertyChanged, ILogRecipient
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

        #endregion

        #region Commands

        public ICommand ExecuteSqlCommand { get; private set; }
        public ICommand ClearLogCommand { get; private set; }
        public ICommand AddConnectionCommand { get; private set; }
        public ICommand CopyLogCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand CloseConnectionCommand { get; private set; }
        public ICommand StopExecutingCommand { get; private set; }
        public ICommand CloseAllConnectionsCommand { get; private set; }

        #endregion

        private readonly Logger logger;
        private readonly IView view;
        private readonly TabController tabController;
        private readonly ConnectionContainer connectionContainer;
        private Thread queryThread;

        public ViewModel(IView view)
        {
            this.view = view;
            StopOnError = true;

            logger = new Logger(this);
            tabController = new TabController(logger);
            connectionContainer = new ConnectionContainer(logger);
            AssignViewEvents();
            InitializeObservables();
            InitializeCommands();
        }

        //subscribe to view events
        private void AssignViewEvents()
        {
            view.OnCloseTabRequested += tabController.CloseTab;
            view.OnCloseAllTabsRequested += tabController.CloseAllTabs;
            view.OnExportTabXMLRequested += tabController.ExportTabAsXml;
            view.OnExportTabCSVRequested += tabController.ExportTabAsCsv;

            view.OnDatabaseChoiceRequested += ChooseDatabase;
            view.OnDatabaseRefreshRequested += RefreshDatabase;
            view.OnSetDatabaseAsCurrentRequested += SetConnectionAsCurrent;
            view.OnDatabaseCloseRequested += CloseDatabaseConnection;
            view.OnCloseAllConnectionsRequested += CloseAllConnections;

            view.OnRoutineSourceRequested += CopyRoutineSource;
        }

        //initialize collections
        private void InitializeObservables()
        {
            DatabasesTree = new ObservableCollection<DatabaseBranch>();
            Connections = new ObservableCollection<IMenuItem> { new MenuItemPlaceholder() };
        }

        //initialize ICommands
        private void InitializeCommands()
        {
            ClearLogCommand = new ActionCommand(logger.Clear);
            CopyLogCommand = new ActionCommand(logger.CopyToClipboard);

            ExecuteSqlCommand = new ActionCommand(ExecuteQuery_OnClick, () => !IsQuerying);
            AddConnectionCommand = new ActionCommand(AddConnection_OnClick);
            CloseConnectionCommand = new ActionCommand(RemoveConnection_OnClick);
            StopExecutingCommand = new ActionCommand(StopExecuting);
            CloseAllConnectionsCommand = new ActionCommand(CloseAllConnections);

            CloseCommand = new ActionCommand(() => Environment.Exit(0));
        }

        //remove connection from collection
        public void RemoveConnection(DatabaseBranch branch)
        {
            DatabasesTree.Remove(branch);
            Connections.Remove(branch.Wrapper);

            if (CurrentConnection == branch.Wrapper)
            {
                CurrentConnection = null;
            }

            if (Connections.Count == 0)
            {
                Connections.Add(new MenuItemPlaceholder());
            }
        }

        //execute given commands
        public void ExecuteQueries(IList<string> queries)
        {
            IsQuerying = true;
            queryThread = new Thread(QueriesExecutionThread);
            queryThread.Start(queries);
        }

        //add new connection
        public bool AddDatabaseConnection(DatabaseConnection newConnection, bool setAsCurrent, DbType databaseType)
        {
            DatabaseConnectionWrapper wrapper = null;

            switch (databaseType)
            {
                case DbType.MySQL:
                    wrapper = new MySqlConnectionWrapper(newConnection);
                    break;
                case DbType.SQLServer:
                    wrapper = new SqlServerConnectionWrapper(newConnection);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
            }

            DatabaseBranch databaseDescription = wrapper.GetDatabaseDescription();
            if (!DatabasesTree.Contains(databaseDescription))
            {
                DatabasesTree.Add(databaseDescription);
                MessageBox.Show($"Succesfully connected to {newConnection.Database} @{newConnection.Server}.", "SQL Pad");

                logger.Write($"New connection: {newConnection}.");

                //remove placeholder as we finally have an element
                if (connections[0].IsPlaceholder)
                {
                    connections.Clear();
                }
                connections.Add(wrapper);
                if (setAsCurrent)
                {
                    ChooseDatabase(wrapper);
                }

                logger.Flush();
                return true;
            }
            else
            {
                MessageBox.Show($"Succesfully connected to {newConnection.Database} @{newConnection.Server}, but this connection already exists.", "SQL Pad");
                return false;
            }
        }

        //thread for executing queries list
        private void QueriesExecutionThread(object queriesObject)
        {
            var queries = (IList<string>)queriesObject;

            var currentDatabaseBranch = DatabasesTree.First(branch => branch.Wrapper == CurrentConnection);
            bool requireRefresh = false;

            string count = queries.Count == 1 ? "query" : "queries";
            logger.Write($"Executing command with {queries.Count} {count}.");

            for (int i = 0; i < queries.Count; i++)
            {
                string query = queries[i];
                QueryType queryType = GetQueryType(i, query);

                bool queryWasSuccesful = PerformQuery(query, queryType);

                if (queryWasSuccesful == false && StopOnError)
                {
                    logger.WriteLine($"\nStopped on query {i + 1} of {queries.Count}!", 1);
                    break;
                }

                if (queryType.RequireDatabaseRefresh())
                {
                    requireRefresh = true;
                }
            }

            //finish
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsQuerying = false; //we are not executing anymore
                logger.Write("\n");
                logger.Flush();
                if (requireRefresh)
                {
                    currentDatabaseBranch = RefreshDatabaseConnection(currentDatabaseBranch);
                }
            }, DispatcherPriority.DataBind);

        }

        private DatabaseBranch RefreshDatabaseConnection(DatabaseBranch currentDatabaseBranch)
        {
            try
            {
                currentDatabaseBranch = CurrentConnection.GetDatabaseDescription();
            }
            catch (DatabaseDroppedException)
            {
                DatabasesTree.Remove(currentDatabaseBranch);
                currentDatabaseBranch.Wrapper.IsChoosen = false;
                CurrentConnection = null;
            }

            return currentDatabaseBranch;
        }

        private bool PerformQuery(string query, QueryType queryType)
        {
            try
            {
                //if true, we need to create new DataTable and add it to TabControl
                if (queryType.YieldsTableOutput())
                {
                    TryExecuteCommandWithOutput(query, queryType);
                }
                else
                {
                    TryExecuteCommandWithNoOutput(query);
                }

                return true;
            }
            catch (Exception err)
            {
                logger.Write($"Error in current query: {err.Message}.", 1);
                return false;
            }
        }

        private QueryType GetQueryType(int index, string query)
        {
            QueryType queryType = QueryType.UNKNOWN;

            try
            {
                //inform user that we recognized their input
                queryType = SQLHelper.GetQueryType(query);
                logger.Write($"{index + 1}) Query type is {queryType}, cool.", 1);
            }
            catch (ArgumentException)
            {
                logger.Write($"{index + 1}) Unrecognized query type {query}, gotta bad feelings 'bout this.", 1);
            }

            return queryType;
        }

        #region Executing SQL Command

        private void TryExecuteCommandWithNoOutput(string query)
        {
            //just execute statement
            int rows = CurrentConnection.ConnectionReference.ExecuteStatement(query);
            logger.Write($"Query successful with {rows} results.", 1);
            Application.Current.Dispatcher.Invoke(() => logger.Flush());
        }

        private void TryExecuteCommandWithOutput(string query, QueryType queryType)
        {
            var result = CurrentConnection.ConnectionReference.PerformSelect(query);

            logger.Write($"Query successful with {result.Data.Count} results.", 1);

            //dispatch new tab
            Application.Current.Dispatcher.Invoke(new Action<QueryType, ResultContainer>((type, container) =>
            {
                tabController.Add(new TabContent(queryType.ToString(), result.ToDataTable()));
                tabController.SelectedTab = Tabs.Back(); //select last tab
                logger.Flush();
            }), DispatcherPriority.DataBind, queryType, result);
        } 

        #endregion

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
            //List<string> queries = new List<string>{ queryText };

            if (queries.Count == 0)
            {
                logger.Write("Ain't no queries here.\n");
                logger.Flush();
            }
            else
            {
                if (tabController.ClearPreviousResults)
                {
                    tabController.CloseAllTabs();
                }
                ExecuteQueries(queries);
            }
        }

        //add new DB connection using DBCollectionDialog
        private void AddConnection_OnClick()
        {
            var dialog = new DatabaseConnectionDialog.DbConnectionDialog();

            if (dialog.ShowDialog() == true)
            {
                AddDatabaseConnection(dialog.Connection, dialog.SetAsCurrent, dialog.DatabaseType);
            }
        }
        
        //remove connection using "Close connection" button
        private void RemoveConnection_OnClick()
        {
            //check if a DatabaseBranch is actually selected
            if (!(view.SelectedTreeItem is DatabaseBranch branch)) return;

            var d = MessageBox.Show($"Remove {branch.DatabaseName} connection?", "SQL Pad", MessageBoxButton.YesNo);
            if (d == MessageBoxResult.Yes)
            {
                RemoveConnection(branch);

                logger.WriteLine($"Removed connection to {branch.DatabaseName}.");
                logger.Flush();
            }
        }

        #endregion

        #region View Event Callbacks

        //choose DB as current
        private void ChooseDatabase(DatabaseConnectionWrapper choosenDatabase)
        {
            foreach (IMenuItem connection in Connections)
            {
                connection.IsChoosen = false;
            }

            choosenDatabase.IsChoosen = true;
            CurrentConnection = choosenDatabase;

            logger.WriteLine($"Set database to {choosenDatabase.Description}.");
            logger.Flush();
        }

        //stop executing current command
        private void StopExecuting()
        {
            if (queryThread != null)
            {
                queryThread.Abort();
                logger.WriteLine("Stopped query thread.");
                logger.Flush();
                queryThread = null;
            }
        }

        //refresh database connection
        private void RefreshDatabase(DatabaseBranch branch)
        {
            try
            {
                DatabasesTree[DatabasesTree.IndexOf(branch)] = branch.Wrapper.GetDatabaseDescription();
                logger.WriteLine($"Refreshed connection to {branch.Wrapper.ConnectionReference}.");
            }
            catch (DatabaseDroppedException)
            {
                logger.WriteLine("Connection closed!");
                CloseDatabaseConnection(branch);
                throw;
            }
            logger.Flush();
        }

        //remove connection using "Set as current" button
        private void SetConnectionAsCurrent(DatabaseBranch branch)
        {
            CurrentConnection.IsChoosen = false;

            CurrentConnection = branch.Wrapper;
            branch.Wrapper.IsChoosen = true;
        }

        //close database connection
        private void CloseDatabaseConnection(DatabaseBranch branch)
        {
            branch.Wrapper.CloseConnection();
            DatabasesTree.Remove(branch);
            logger.WriteLine($"Closed connection to {branch.Wrapper.ConnectionReference}.");
            logger.Flush();
        }

        //copy routine source code to clipboard
        private void CopyRoutineSource(Routine routine)
        {
            Clipboard.SetText(routine.Code);
            logger.WriteLine("Log has been copied to clipboard.");
            logger.Flush();
        }

        //close all connections...
        private void CloseAllConnections()
        {
            CurrentConnection = null;
            DatabasesTree.Clear();
            Connections.Clear();
            Connections.Add(new MenuItemPlaceholder());
            logger.WriteLine("Closed all connections.");
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
