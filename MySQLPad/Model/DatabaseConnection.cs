using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Model
{

    public enum DbType
    {
        MySQL,
        SQLServer,
    }

    public abstract class DatabaseConnection : ImplementsPropertyChanged, IDisposable, IMenuItem
    {
        #region IMenuItem Members

        protected bool isChoosen;
        public bool IsChoosen
        {
            get => isChoosen;
            set => Set(ref isChoosen, value);
        }

        public bool IsPlaceholder => false;

        protected string description;
        public string Description
        {
            get => description;
            set => Set(ref description, value);
        }

        #endregion

        protected bool isAvailable;
        public bool IsAvailable
        {
            get => isAvailable;
            set => Set(ref isAvailable, value);
        }

        private DbType databaseType;
        public DbType DatabaseType
        {
            get => databaseType;
            set => Set(ref databaseType, value);
        } 

        public string Server { get; protected set; }
        public string Database { get; protected set; }
        public string UserId { get; protected set; }
        protected string password;

        public string LastError { get; protected set; }
        public int LastErrorCode { get; protected set; }

        protected ExternalTimeDispatcher connectionCheck;
        protected const string tableType_View = "VIEW";

        protected DbConnection connection;


        protected abstract DbCommand CreateCommand(string query);

        protected abstract void CreateConnection();

        public abstract bool Ping();

        public abstract List<ColumnDescription> GetTableDescription(string tableName);

        public abstract DatabaseBranch GetDatabaseDescription();


        public virtual bool CheckAvailability() {
            bool canPing;
            try
            {
                canPing = Ping();
            }
            catch (DbException)
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

        public virtual bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (DbException ex)
            {
                switch (ex.ErrorCode)
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
                        LastErrorCode = ex.ErrorCode;
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

        public virtual bool CloseConnection(bool disableDispatcher = false)
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
            catch (DbException)
            {
                return false;
            }
        }

        public virtual ResultContainer Select(string query)
        {
            if (!CheckAvailability() && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            List<string[]> results = new List<string[]>();

            if (string.IsNullOrEmpty(query))
            {
                return new ResultContainer();
            }

            var cmd = CreateCommand(query);
            List<DbColumn> schema;
            using (DbDataReader dataReader = cmd.ExecuteReader())
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

        public virtual int ExecuteStatement(string query)
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }
            ;
            var cmd = CreateCommand(query);
            return cmd.ExecuteNonQuery();
        }

        public virtual void Dispose()
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