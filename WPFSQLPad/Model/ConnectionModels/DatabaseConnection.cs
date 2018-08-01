using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Model.ConnectionModels
{

    public enum DbType
    {
        MySQL,
        SQLServer,
    }

    public abstract class DatabaseConnection : IDisposable
    {
        public string Description { get; protected set; }
        public bool IsAvailable { get; protected set; }
        public DbType DatabaseType { get; protected set; }
        public string Delimiter { get; protected set; }

        public bool IsPerformingQuery { get; set; }

        public string Server { get; protected set; }
        public string Database { get; protected set; }
        public string UserId { get; protected set; }
        protected string password;

        public string LastError { get; protected set; }
        public int LastErrorCode { get; protected set; }

        protected ExternalTimeDispatcher connectionCheck;
        public const string tableType_View = "VIEW";

        protected DbConnection connection;

        protected DatabaseConnection(string server, string database, string userId, string password, DbType databaseType)
        {
            Server = server;
            Database = database;
            UserId = userId;
            this.password = password;
            DatabaseType = databaseType;

            Delimiter = ";";
        }

        protected abstract void CreateConnection();

        public abstract bool Ping();

        public virtual bool CheckAvailability()
        {
            if (connection.State != ConnectionState.Open)
            {
                return false;
            }

            if (IsPerformingQuery)
            {
                return true;
            }

            bool canPing;
            try
            {
                canPing = Ping();
            }
            catch (DbException e)
            {
                Debug.WriteLine($"CheckAvailablity: {e.Message} {e.ErrorCode}");
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
                HandleError(ex);
                return false;
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine($"InvalidOperationException in open: (VM): {e.Message}, code: {e}", "SQL Pad");
                return true;
            }
        }

        private void HandleError(DbException ex)
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
                case -2147467259:
                    LastError = "Cannot find database";
                    LastErrorCode = 1045;
                    break;
                default:
                    LastError = "Unknown";
                    LastErrorCode = ex.ErrorCode;
                    break;
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
            catch (DbException e)
            {
                Debug.WriteLine($"DBexc in open: (VM): {e.Message}, code: {e}", "SQL Pad");
                return false;
            }
        }

        public virtual ResultContainer PerformSelect(string query)
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            IsPerformingQuery = true;

            List<string[]> results = new List<string[]>();

            if (string.IsNullOrEmpty(query))
            {
                return new ResultContainer();
            }

            var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            Debug.Assert(cmd.Connection == connection);
            List<DbColumn> schema;

            try
            {
                using (DbDataReader dataReader = cmd.ExecuteReader())
                {
                    schema = new List<DbColumn>(dataReader.GetColumnSchema());

                    while (dataReader.Read())
                    {
                        results.Add(new string[schema.Count]);
                        for (int i = 0; i < schema.Count; i++)
                        {
                            //assign data to last added element
                            var result = dataReader[schema[i].ColumnName];
                            results[results.Count - 1][i] = GetData(result);
                        }
                    }

                    dataReader.Close();
                }
            }
            catch (DbException e)
            {
                if (e.Message.Equals("No database selected"))
                {
                    throw new DatabaseDroppedException();
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                IsPerformingQuery = false;
            }

            return new ResultContainer("Query result", Database, schema, results);
        }

        public virtual int ExecuteStatement(string query)
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            IsPerformingQuery = true;
            var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            int rowCount = cmd.ExecuteNonQuery();
            IsPerformingQuery = false;

            return rowCount;
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

        protected string GetData(object result)
        {
            if (result is byte[] bytes)
            {
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            else
            {
                return result.ToString();
            }
        }

    }
}