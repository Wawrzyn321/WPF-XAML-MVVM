using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace Model
{
    public sealed class SQLServerDatabaseConnection : DatabaseConnection
    {
        private SqlConnection connection;

        public SQLServerDatabaseConnection(string server, string database, string userId, string password)
        {
            Server = server;
            Database = database;
            UserId = userId;
            this.password = password;

            Description = $"{server}: {database} (SQL Server)";
            DatabaseType = DbType.SQLServer;

            CreateConnection();

            connectionCheck = new ExternalTimeDispatcher(this);
        }

        protected override void CreateConnection()
        {
            string connectionString =
                $"Data Source={Server};Initial Catalog={Database};User ID={UserId};Password={password}";

            connection = new SqlConnection(connectionString);

            IsAvailable = OpenConnection();
        }

        public override bool CheckAvailability()
        {
            return true; //todo

            bool canPing;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                var command = new SqlCommand("SELECT 1", connection);
                var r = command.ExecuteScalar();
                canPing = (int)r == 1;
            }
            catch (SqlException)
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

        public override bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    default:
                        LastError = "Unknown " + ex.Message;
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

        public override bool CloseConnection(bool disableDispatcher = false)
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
            catch (SqlException)
            {
                return false;
            }
        }

        public override DatabaseBranch GetDatabaseDescription()
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            ResultContainer allTables = Select("SELECT * FROM INFORMATION_SCHEMA.TABLES");
            var tableNames = new string[allTables.Data.Count];
            var tableTypes = new string[allTables.Data.Count];
            for (int i = 0; i < allTables.Data.Count;i++)
            {
                tableNames[i] = allTables.Data[i][2];
                tableTypes[i] = allTables.Data[i][3];
            }


            var tables = new ObservableCollection<TableBranch>();
            var views = new ObservableCollection<TableBranch>();
            for (int i = 0; i < tableNames.Length; i++)
            {
                ResultContainer s = Select("SP_COLUMNS testowaTabela");

                List<ColumnDescription> columns = GetTableDescription(tableNames[i]);
                bool isView = tableTypes[i] == tableType_View;
                if (isView)
                {
                    views.Add(new TableBranch(tableNames[i], columns));
                }
                else
                {
                    tables.Add(new TableBranch(tableNames[i], columns));
                }
            }
            return new DatabaseBranch($"{Server}:{Database}", tables, views, this);

        }

        public override List<ColumnDescription> GetTableDescription(string tableName)
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            List<ColumnDescription> result = new List<ColumnDescription>();

            foreach (string[] s in Select($"SP_COLUMNS '{tableName}'").Data)
            {
                result.Add(new ColumnDescription
                {
                    Name = s[3],
                    Type = s[5],
                    CanBeNull = s[10].Equals("0"),
                    Key = "?",
                    Extra = "?",
                    Default = "?",
                });
            }

            return result;
        }

        public override ResultContainer Select(string query)
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

            var cmd = new SqlCommand(query, connection);
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

        public override int ExecuteStatement(string query)
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            SqlCommand cmd = new SqlCommand(query, connection);
            int rows = cmd.ExecuteNonQuery();
            return rows;
        }

        public override void Dispose()
        {
            connectionCheck.Stop();
            CloseConnection();
            connection?.Dispose();
        }
    }
}
