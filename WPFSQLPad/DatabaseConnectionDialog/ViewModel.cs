using System;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows;
using Model;


namespace DatabaseConnectionDialog
{
    /// <summary>
    /// Simple ViewModel for connection dialog.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {

        #region Observed Properties

        private DbType databaseType;
        public DbType DatabaseType
        {
            get => databaseType;
            set => Set(ref databaseType, value);
        }

        private string server;
        public string Server
        {
            get => server;
            set => Set(ref server, value);
        }

        private string database;
        public string Database
        {
            get => database;
            set => Set(ref database, value);
        }

        private string userID;
        public string UserID
        {
            get => userID;
            set => Set(ref userID, value);
        }

        private bool setAsCurrent;
        public bool SetAsCurrent
        {
            get => setAsCurrent;
            set => Set(ref setAsCurrent, value);
        }

        #endregion

        private readonly IView view;

        public ViewModel(IView view)
        {
            this.view = view;
            view.OnConnectButtonClicked += TryToConnect;

            Server = "localhost";
            Database = "testowa";
            UserID = "root";

            SetAsCurrent = true;
        }

        private void TryToConnect(SecureString str)
        {
            if (!CheckForEmptyInput())
            {
                return;
            }

            try
            {
                DatabaseConnection connection = ConnectFucker(str);

                if (connection.IsAvailable == false)
                {
                    connection.CloseConnection(true);
                    MessageBox.Show($"Could not connect to database, reason: {connection.LastError} (code: {connection.LastErrorCode}).", "SQL Pad");
                    return;
                }

                view.ReturnToCaller(connection, SetAsCurrent);
            }
            catch (MySqlException e)
            {
                //handle some know error codes
                switch (e.ErrorCode)
                {
                    case 0:
                        MessageBox.Show($"Access denied for user {UserID}!", "SQL Pad");
                        break;
                    case -2147467259:
                        MessageBox.Show("Unknown database or server!", "SQL Pad");
                        break;
                    default:
                        MessageBox.Show($"Unknown error: {e.Message}, code: {e.ErrorCode}", "SQL Pad");
                        break;
                }
            }
        }

        private DatabaseConnection ConnectFucker(SecureString str)
        {
            if (DatabaseType == DbType.MySQL)
            {
                return new MySQLDatabaseConnection(Server, Database, UserID,
                    SecureStringUtility.SecureStringToString(str));
            }
            else if (DatabaseType == DbType.SQLServer)
            {
                return new SQLServerDatabaseConnection(Server, Database, UserID,
                    SecureStringUtility.SecureStringToString(str));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private bool CheckForEmptyInput()
        {
            if (string.IsNullOrEmpty(Server))
            {
                MessageBox.Show("You have to fill the Server box!", "SQL Pad");
                return false;
            }

            if (string.IsNullOrEmpty(Database))
            {
                MessageBox.Show("You have to fill the Database box!", "SQL Pad");
                return false;
            }

            if (string.IsNullOrEmpty(userID))
            {
                MessageBox.Show("You have to fill the User ID box!", "SQL Pad");
                return false;
            }

            return true;
        }
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool Set<T>(ref T oldValue, T value, [CallerMemberName] string propertyName = null)
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