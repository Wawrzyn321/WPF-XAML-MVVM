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

        protected override DbCommand CreateCommand(string query)
        {
            return new SqlCommand(query, (SqlConnection)connection);
        }

        protected override void CreateConnection()
        {
            string connectionString =
                $"Data Source={Server};Initial Catalog={Database};User ID={UserId};Password={password}";

            connection = new SqlConnection(connectionString);

            IsAvailable = OpenConnection();
        }

        public override bool Ping()
        {
            //todo
            return true;
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

    }
}
