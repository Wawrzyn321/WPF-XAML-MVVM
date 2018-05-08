using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using Model.TreeItems;
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
            var tables = new List<TableBranch>();
            var views = new List<TableBranch>();
            foreach (var data in s.Data)
            {
                string tableName = data[0];
                List<ColumnDescription> columns = GetTableDescription(tableName);

                //add to separate tables, depending on type
                bool isView = data[1] == tableType_View;
                if (isView)
                {
                    views.Add(new TableBranch(tableName, columns, this));
                }
                else
                {
                    tables.Add(new TableBranch(tableName, columns, this));
                }
            }

            var routines = GetRoutines();

            return new DatabaseBranch($"{Server}:{Database}", tables, views, routines, this);
        }

        public override List<Routine> GetRoutines()
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            ResultContainer s = Select("SHOW FUNCTION STATUS");
            List<Routine> routines = new List<Routine>(s.Data.Count);

            foreach (string[] data in s.Data)
            {
                string name = data[1];
                string type = data[2];
                string[] query =
                    Select($"SELECT param_list, returns FROM mysql.proc WHERE db = '{Database}' AND name = '{name}'").Data[0];
                string parameters = query[0];
                string returnType = query[1];
                routines.Add(new Routine(name, type, parameters, returnType, this));
            }

            return routines;
        }

        public override string GetRoutineCode(Routine.RoutineType type, string name)
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            return Select($"SHOW CREATE {type} {name}").Data[0][2];
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
                string Name = s[0];
                string Type = s[1];
                bool CanBeNull = s[2].Equals(ColumnDescription.CanBeNull_Yes);
                string Key = s[3];
                string Extra = s[4];
                string Default = s[5];

                result.Add(new ColumnDescription(Name, Type, CanBeNull, Key, Default, Extra, this));
            }

            return result;
        }

    }
}
