using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace Model
{

    /// <summary>
    /// Handler for database connection.
    /// </summary>
    public sealed class MySQLDatabaseConnection : DatabaseConnection
    {

        public MySQLDatabaseConnection(string server, string database, string userId, string password)
        {
            Server = server;
            Database = database;
            UserId = userId;
            this.password = password;

            Description = $"{server}: {database} (MySQL)";
            DatabaseType = DbType.MySQL;

            CreateConnection();

            connectionCheck = new ExternalTimeDispatcher(this);
        }

        protected override DbCommand CreateCommand(string query)
        {
            return new MySqlCommand(query, (MySqlConnection)connection);
        }

        protected override void CreateConnection()
        {
            string connectionString = $"server={Server};database={Database};uid={UserId};pwd={password};sslmode=none";
            connection = new MySqlConnection(connectionString);

            IsAvailable = OpenConnection();
        }

        public override bool Ping()
        {
            return ((MySqlConnection) connection).Ping();
        }

        public override DatabaseBranch GetDatabaseDescription()
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

        public override List<ColumnDescription> GetTableDescription(string tableName)
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

    }
}
