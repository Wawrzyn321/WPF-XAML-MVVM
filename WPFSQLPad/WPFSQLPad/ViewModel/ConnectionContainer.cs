using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Model;
using Model.ConnectionModels;
using QueryTabControl;
using WPFSQLPad.ConnectionWrappers;
using WPFSQLPad.IMenuItems;
using WPFSQLPad.TreeItems;
using WPFSQLPad.View;

namespace WPFSQLPad.ViewModel
{
   public class ConnectionContainer : INotifyPropertyChanged
    {
        private ObservableCollection<IMenuItem> connections;
        public ObservableCollection<IMenuItem> Connections
        {
            get => connections;
            set => Set(ref connections, value);
        }

        private DatabaseConnectionWrapper currentConnection;
        public DatabaseConnectionWrapper CurrentConnection
        {
            get => currentConnection;
            set => Set(ref currentConnection, value);
        }

        private ObservableCollection<DatabaseBranch> databasesTree;
        public ObservableCollection<DatabaseBranch> DatabasesTree
        {
            get => databasesTree;
            set => Set(ref databasesTree, value);
        }

        private bool stopOnError;
        public bool StopOnError
        {
            get => stopOnError;
            set => Set(ref stopOnError, value);
        }

        private bool isQuerying;
        public bool IsQuerying
        {
            get => isQuerying;
            set => Set(ref isQuerying, value);
        }

        private readonly Logger.LoggerViewModel logger;
        private readonly TabController tabController;
        private Thread queryThread;

        public ConnectionContainer(Logger.LoggerViewModel logger, TabController tabController)
        {
            this.logger = logger;
            this.tabController = tabController;
            DatabasesTree = new ObservableCollection<DatabaseBranch>();
            Connections = new ObservableCollection<IMenuItem> { new MenuItemPlaceholder() };
            StopOnError = true;
        }

        public void AddConnection(DatabaseConnectionWrapper wrapper, bool setAsCurrent)
        {
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

            logger.WriteLine($"Removed connection to {branch.DatabaseName}.");
        }

        //refresh database connection
        public void RefreshDatabase(DatabaseBranch branch)
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
        }

        //remove connection using "Set as current" button
        public void SetConnectionAsCurrent(DatabaseBranch branch)
        {
            if (CurrentConnection != null)
            {
                CurrentConnection.IsChoosen = false;
            }

            CurrentConnection = branch.Wrapper;
            branch.Wrapper.IsChoosen = true;
        }
        
        //close all connections...
        public void CloseAllConnections()
        {
            CurrentConnection = null;
            DatabasesTree.Clear();
            Connections.Clear();
            Connections.Add(new MenuItemPlaceholder());
            logger.WriteLine("Closed all connections.");
        }

        //choose DB as current
        public void ChooseDatabase(DatabaseConnectionWrapper choosenDatabase)
        {
            foreach (IMenuItem connection in Connections)
            {
                connection.IsChoosen = false;
            }

            choosenDatabase.IsChoosen = true;
            CurrentConnection = choosenDatabase;

            logger.WriteLine($"Set database to {choosenDatabase.Description}.");
        }

        //close database connection
        public void CloseDatabaseConnection(DatabaseBranch branch)
        {
            branch.Wrapper.CloseConnection();
            DatabasesTree.Remove(branch);
            logger.WriteLine($"Closed connection to {branch.Wrapper.ConnectionReference}.");
        }

        //copy routine source code to clipboard
        public void CopyRoutineSource(Routine routine)
        {
            Clipboard.SetText(routine.Code);
            logger.WriteLine("Log has been copied to clipboard.");
        }

        //try to refresh DB connection
        public DatabaseBranch RefreshDatabaseConnection(DatabaseBranch currentDatabaseBranch)
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

                AddConnection(wrapper, setAsCurrent);

                return true;
            }
            else
            {
                MessageBox.Show($"Succesfully connected to {newConnection.Database} @{newConnection.Server}, but this connection already exists.", "SQL Pad");
                return false;
            }
        }

        //stop executing current command
        public void StopExecuting()
        {
            if (queryThread != null)
            {
                queryThread.Abort();
                logger.WriteLine("Stopped query thread.");
                queryThread = null;
            }
        }

        //execute given commands
        public void ExecuteQueries(IList<string> queries)
        {
            IsQuerying = true;
            queryThread = new Thread(QueriesExecutionThread);
            queryThread.Start(queries);
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
                if (requireRefresh)
                {
                    currentDatabaseBranch = RefreshDatabaseConnection(currentDatabaseBranch);
                }
            }, DispatcherPriority.DataBind);

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
            Application.Current.Dispatcher.Invoke(() => logger.Write($"Query successful with {rows} results.", 1));
        }

        private void TryExecuteCommandWithOutput(string query, QueryType queryType)
        {
            var result = CurrentConnection.ConnectionReference.PerformSelect(query);

            logger.Write($"Query successful with {result.Data.Count} results.", 1);

            //dispatch new tab
            Application.Current.Dispatcher.Invoke(new Action<QueryType, ResultContainer>((type, container) =>
            {
                tabController.Add(new TabContent(queryType.ToString(), result.ToDataTable()));
                tabController.SelectedTab = tabController.Tabs.Back(); //select last tab
            }), DispatcherPriority.DataBind, queryType, result);
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
