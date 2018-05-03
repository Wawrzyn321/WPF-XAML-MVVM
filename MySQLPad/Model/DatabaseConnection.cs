using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;

//https://github.com/JaCraig/SQLParser/blob/master/LICENSE

namespace Model
{

    /// <summary>
    /// Handler for database connection.
    /// </summary>
    public class DatabaseConnection : ImplementsPropertyChanged, IDisposable, IMenuItem
    {
        #region IMenuItem Members

        private bool isChoosen;
        public bool IsChoosen
        {
            get => isChoosen;
            set => Set(ref isChoosen, value);
        }

        public bool IsPlaceholder => false;

        private string description;
        public string Description
        {
            get => description;
            set => Set(ref description, value);
        }

        #endregion

        private bool isAvailable;
        public bool IsAvailable
        {
            get => isAvailable;
            set => Set(ref isAvailable, value);
        } 
        
        private MySqlConnection connection;
        public string Server { get; }
        public string Database { get; }
        public string UserId { get; }
        private readonly string password;

        public string LastError { get; private set; }
        public int LastErrorCode { get; private set; }
        
        private readonly ExternalTimeDispatcher connectionCheck;
        private const string tableType_View = "VIEW";

        public DatabaseConnection(string server, string database, string userId, string password)
        {
            Server = server;
            Database = database;
            UserId = userId;
            this.password = password;

            Description = $"{server}: {database}";

            CreateConnection();

            connectionCheck = new ExternalTimeDispatcher(this);
        }

        private void CreateConnection()
        {
            string connectionString = $"server={Server};database={Database};uid={UserId};pwd={password};sslmode=none";
            connection = new MySqlConnection(connectionString);

            IsAvailable = OpenConnection();
        }

        public bool CheckAvailability()
        {
            bool canPing;
            try
            {
                canPing = connection.Ping();
            }
            catch (MySqlException)
            {
                //ignore
                return false;
            }

            //try to reconnect
            if (!canPing)
            {
                connection.Close();
                IsAvailable = OpenConnection();
            }

            return IsAvailable;
        }

        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                    case 1042:
                        LastError = "Cannot connect to server.  Contact administrator";
                        LastErrorCode = 0;
                        break;
                    case 1045:
                        LastError = "Invalid username/password, please try again";
                        LastErrorCode = 1045;
                        break;
                    default:
                        LastError = "Unknown";
                        LastErrorCode = ex.Number;
                        break;
                }
                return false;
            }
            catch (InvalidOperationException)
            {
                //ignore
                return true;
            }
        }

        public bool CloseConnection(bool disableDispatcher = false)
        {
            if (disableDispatcher)
            {
                connectionCheck.Stop();
            }

            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public DatabaseBranch GetDatabaseDescription()
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            ResultContainer s = Select("SHOW FULL TABLES");
            var tables = new ObservableCollection<TableBranch>();
            var views = new ObservableCollection<TableBranch>();
            foreach (var data in s.Data)
            {
                string tableName = data[0];
                List<ColumnDescription> columns = GetTableDescription(tableName);

                //add to separate tables, depending on type
                bool isView = data[1] == tableType_View;
                if (isView)
                {
                    views.Add(new TableBranch(tableName, columns));
                }
                else
                {
                    tables.Add(new TableBranch(tableName, columns));
                }
            }

            return new DatabaseBranch($"{Server}:{Database}", tables, views, this);
        }

        public List<ColumnDescription> GetTableDescription(string tableName)
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            List<ColumnDescription> result = new List<ColumnDescription>();

            foreach (string[] s in Select($"DESC {tableName}").Data)
            {
                result.Add(new ColumnDescription
                {
                    Name = s[0],
                    Type = s[1],
                    CanBeNull = s[2].Equals(ColumnDescription.CanBeNull_Yes),
                    Key = s[3],
                    Extra = s[4],
                    Default = s[5],
                });
            }

            return result;
        }

        //DELETE, UPDATE and DELETE statements (Select also, but won't return any records)
        public int ExecuteStatement(string query)
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            MySqlCommand cmd = new MySqlCommand(query, connection);
            int rows = cmd.ExecuteNonQuery();
            return rows;
        }

        //SELECT
        public ResultContainer Select(string query)
        {
            if (!CheckAvailability() && connection.State!=ConnectionState.Open)
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            List<string[]> results = new List<string[]>();

            if (string.IsNullOrEmpty(query))
            {
                return new ResultContainer();
            }

            MySqlCommand cmd = new MySqlCommand(query, connection);
            List<DbColumn> schema;
            using (MySqlDataReader dataReader = cmd.ExecuteReader())
            {
                schema = new List<DbColumn>(dataReader.GetColumnSchema());

                while (dataReader.Read())
                {
                    results.Add(new string[schema.Count]);
                    for (int i = 0; i < schema.Count; i++)
                    {
                        //assign data to last added element
                        results[results.Count - 1][i] = dataReader[schema[i].ColumnName].ToString();
                    }   
                }

                dataReader.Close();
            }

            return new ResultContainer("Query result", Database, schema, results);
        }

        public void Dispose()
        {
            connectionCheck.Stop();
            CloseConnection();
            connection?.Dispose();
        }

        public override string ToString()
        {
            return $"DatabaseConnection to {Server} at database {Database}, user: {UserId}";
        }
    }

}
