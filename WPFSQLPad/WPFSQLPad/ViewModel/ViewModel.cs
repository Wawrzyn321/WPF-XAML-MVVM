using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Model;
using Model.ConnectionModels;
using WPFSQLPad.ConnectionWrappers;
using WPFSQLPad.TreeItems;
using WPFSQLPad.View;

namespace WPFSQLPad.ViewModel
{

    /// <summary>
    /// ViewModel for main application
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        #region Observed properties

        private ObservableCollection<TabContent> tabs;
        public ObservableCollection<TabContent> Tabs
        {
            get => tabs;
            set => Set(ref tabs, value);
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

        private TabContent selectedTab;
        public TabContent SelectedTab
        {
            get => selectedTab;
            set => Set(ref selectedTab, value);
        }

        private bool clearPreviousResults;
        public bool ClearPreviousResults
        {
            get => clearPreviousResults;
            set => Set(ref clearPreviousResults, value);
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

        #endregion

        #region Commands

        public ICommand ExecuteSqlCommand { get; private set; }
        public ICommand ClearLogCommand { get; private set; }
        public ICommand AddConnectionCommand { get; private set; }
        public ICommand CopyLogCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand RemoveConnectionCommand { get; private set; }
        public ICommand StopExecutingCommand { get; private set; }

        #endregion

        private readonly Logger logger;
        private readonly IView view;
        private Thread queryThread;

        public ViewModel(IView view)
        {
            this.view = view;
            StopOnError = true;

            logger = new Logger();
            AssignViewEvents();
            InitializeObservables();
            InitializeCommands();
        }

        //subscribe to view events
        private void AssignViewEvents()
        {
            view.OnExportTabXMLRequested += ExportTabAsXML;
            view.OnExportTabCSVRequested += ExportTabAsCSV;
            view.OnCloseTabRequested += CloseTab;
            view.OnDatabaseChoiceRequested += ChooseDatabase;
            view.OnCloseAllTabsRequested += CloseAllTabs;
            view.OnDatabaseRefreshRequested += RefreshDatabase;
            view.OnDatabaseCloseRequested += CloseDatabaseConnection;
            view.OnRoutineSourceRequested += CopyRoutineSource;
        }

        //initialize collections
        private void InitializeObservables()
        {
            Tabs = new ObservableCollection<TabContent>();
            DatabasesTree = new ObservableCollection<DatabaseBranch>();
            Connections = new ObservableCollection<IMenuItem> { new MenuItemPlaceholder() };
        }

        //initialize ICommands
        private void InitializeCommands()
        {
            ExecuteSqlCommand = new ActionCommand(Execute_OnClick, () => !IsQuerying);
            ClearLogCommand = new ActionCommand(ClearLog_OnClick);
            AddConnectionCommand = new ActionCommand(AddConnection_OnClick);
            CopyLogCommand = new ActionCommand(CopyLog_OnClick);
            CloseCommand = new ActionCommand(() => Environment.Exit(0));
            RemoveConnectionCommand = new ActionCommand(RemoveConnection_OnClick);
            StopExecutingCommand = new ActionCommand(StopExecuting);
        }

        //remove connection from collection
        public void RemoveConnection(DatabaseBranch branch)
        {
            DatabasesTree.Remove(branch);
            Connections.Remove(branch.ConnectionReference);

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

                Log = logger.Flush();
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

            var currentDatabaseBranch = DatabasesTree.First(branch => branch.ConnectionReference == CurrentConnection);
            int index = DatabasesTree.IndexOf(currentDatabaseBranch);
            bool requireRefresh = false;

            string count = queries.Count == 1 ? "query" : "queries";
            logger.Write($"Executing command with {queries.Count} {count}.");

            for (int i = 0; i < queries.Count; i++)
            {
                string query = queries[i];
                QueryType queryType = QueryType.UNKNOWN;
                
                try
                {
                    //inform user that we recognized their input
                    queryType = SQLHelper.GetQueryType(query);
                    logger.Write($"{i + 1}) Query type is {queryType}, cool.", 1);
                }
                catch (ArgumentException)
                {
                    logger.Write($"{i + 1}) Unrecognized query type {query}, gotta bad feelings 'bout this.", 1);
                }


                //if true, we need to create new DataTable and add it to TabControl
                if (queryType.YieldsTableOutput())
                {
                    try
                    {
                        TryExecuteCommandWithOutput(query, queryType);
                    }
                    catch (Exception err)
                    {
                        logger.Write($"Error in current query: {err.Message}.", 1);

                        if (StopOnError)
                        {
                            logger.WriteLine($"\nStopped on query {i+1} of {queries.Count}!", 1);
                            break;
                        }
                    }
                }
                else
                {
                    try
                    {
                        TryExecuteCommandWithNoOutput(query);
                    }
                    catch (Exception err)
                    {
                        logger.Write($"Error in current query: {err.Message}.", 1);

                        if (StopOnError)
                        {
                            logger.WriteLine($"\nStopped on query {i + 1} of {queries.Count}!", 1);
                            break;
                        }
                    }
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
                Log = logger.Flush();
                if (requireRefresh)
                {
                    try
                    {
                        DatabasesTree[index] = CurrentConnection.GetDatabaseDescription();
                    }
                    catch (DatabaseDroppedException)
                    {
                        DatabasesTree.RemoveAt(index);
                        foreach (IMenuItem connection in Connections)
                        {
                            connection.IsChoosen = false;
                        }

                        CurrentConnection = null;
                    }
                }
            }, DispatcherPriority.DataBind);

        }

        #region Executing SQL Command
        
        private void TryExecuteCommandWithNoOutput(string query)
        {
            //just execute statement
            int rows = CurrentConnection.connectionReference.ExecuteStatement(query);
            logger.Write($"Query successful with {rows} results.", 1);
            Application.Current.Dispatcher.Invoke(() => Log = logger.Flush());
        }

        private void TryExecuteCommandWithOutput(string query, QueryType queryType)
        {
            ResultContainer result = CurrentConnection.connectionReference.PerformSelect(query);

            logger.Write($"Query successful with {result.Data.Count} results.", 1);

            //dispatch new tab
            Application.Current.Dispatcher.Invoke(new Action<QueryType, ResultContainer>((type, container) =>
            {
                Tabs.Add(new TabContent(queryType.ToString(), result.ToDataTable()));
                SelectedTab = Tabs.Back(); //select last tab
                Log = logger.Flush();
            }), DispatcherPriority.DataBind, queryType, result);
        } 

        #endregion

        #region Command Callbacks

        //execute command
        private void Execute_OnClick()
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
                Log = logger.Flush();
            }
            else
            {
                if (ClearPreviousResults)
                {
                    Tabs.Clear();
                }
                ExecuteQueries(queries);
            }
        }

        //clear log
        private void ClearLog_OnClick()
        {
            logger.Clear();
            Log = logger.Flush();
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

        //copy log to clipboard
        private void CopyLog_OnClick()
        {
            Clipboard.SetText(Log);
            logger.WriteLine("Log has been copied to clipboard.");
            Log = logger.Flush();
        }

        //remove connection using "Remove connection" button
        private void RemoveConnection_OnClick()
        {
            //check if a DatabaseBranch is actually selected
            if (!(view.SelectedTreeItem is DatabaseBranch branch)) return;

            var d = MessageBox.Show($"Remove {branch.DatabaseName} connection?", "SQL Pad", MessageBoxButton.YesNo);
            if (d == MessageBoxResult.Yes)
            {
                RemoveConnection(branch);

                logger.WriteLine($"Removed connection to {branch.DatabaseName}.");
                Log = logger.Flush();
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
            Log = logger.Flush();
        }

        //close given result tab
        private void CloseTab(TabContent content)
        {
            Tabs.Remove(content);
        }

        //export to XML
        private void ExportTabAsXML(TabContent tabContent)
        {
            if (DataTableSerializer.SerializeAsXML(tabContent.Data))
            {
                logger.WriteLine("\nSaved the table as XML.");
            }
            else
            {
                logger.WriteLine("\nCould not save XML file!.");
            }
            Log = logger.Flush();
        }

        //export to CSV
        private void ExportTabAsCSV(TabContent tabContent)
        {
            if (DataTableSerializer.SerializeAsCSV(tabContent.Data))
            {
                logger.WriteLine("\nSaved the table as CSV.");
            }
            else
            {
                logger.WriteLine("\nCould not save CSV file!.");
            }
            Log = logger.Flush();
        }

        //close all result tabs
        private void CloseAllTabs()
        {
            Tabs.Clear();
            logger.WriteLine("Closed all tabs.");
            Log = logger.Flush();
        }

        //stop executing current command
        private void StopExecuting()
        {
            if (queryThread != null)
            {
                queryThread.Abort();
                logger.WriteLine("Stopped query thread.");
                Log = logger.Flush();
                queryThread = null;
            }
        }

        //refresh database connection
        private void RefreshDatabase(DatabaseBranch branch)
        {
            try
            {
                DatabasesTree[DatabasesTree.IndexOf(branch)] = branch.ConnectionReference.GetDatabaseDescription();
                logger.WriteLine($"Refreshed connection to {branch.ConnectionReference.connectionReference}.");
            }
            catch (DatabaseDroppedException)
            {
                logger.WriteLine("Connection closed!");
                CloseDatabaseConnection(branch);
                throw;
            }
            Log = logger.Flush();
        }

        //close database connection
        private void CloseDatabaseConnection(DatabaseBranch branch)
        {
            branch.ConnectionReference.connectionReference.CloseConnection(true);
            DatabasesTree.Remove(branch);
            logger.WriteLine($"Closed connection to {branch.ConnectionReference.connectionReference}.");
            Log = logger.Flush();
        }

        //copy routine source code to clipboard
        private void CopyRoutineSource(Routine routine)
        {
            Debug.WriteLine("!");
            Clipboard.SetText(routine.GetCode());
            logger.WriteLine("Log has been copied to clipboard.");
            Log = logger.Flush();
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
