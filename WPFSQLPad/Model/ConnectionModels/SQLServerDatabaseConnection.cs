﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Model.TreeItems;

namespace Model.ConnectionModels
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

            var tables = new List<TableBranch>();
            var views = new List<TableBranch>();
            for (int i = 0; i < tableNames.Length; i++)
            {
                ResultContainer s = Select("SP_COLUMNS testowaTabela");

                List<ColumnDescription> columns = GetTableDescription(tableNames[i]);
                bool isView = tableTypes[i] == tableType_View;
                if (isView)
                {
                    views.Add(new TableBranch(tableNames[i], columns, this));
                }
                else
                {
                    tables.Add(new TableBranch(tableNames[i], columns, this));
                }
            }


            var routines = GetRoutines();

            return new DatabaseBranch($"{Server}: {Database}", tables, views, routines, this);
        }

        public override List<Routine> GetRoutines()
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            var routines = new List<Routine>();
            var data = Select($"select ROUTINE_NAME, ROUTINE_TYPE from {Database}.information_schema.routines").Data;
            foreach (var routineData in data)
            {
                string name = routineData[0];
                string type = routineData[1];

                List<string[]> parametersData =
                    Select(
                        $"select name, TYPE_NAME(system_type_id)  from sys.parameters where object_id = object_id('{name}')").Data;

                string[] parameters = new string[parametersData.Count];
                for (int i = 0; i < parametersData.Count; i++)
                {
                    parameters[i] = $"{parametersData[i][1]} {parametersData[i][0]}";
                }

                routines.Add(new Routine(name, type, string.Join(", ", parameters), string.Empty, this));
            }

            return routines;
        }

        public override string GetRoutineCode(Routine.RoutineType type, string name)
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Database is unavailable!");
            }

            string typeUppercase = type.ToString().ToUpper();
            var result = Select($"select ROUTINE_DEFINITION from {Database}.information_schema.routines WHERE ROUTINE_TYPE='{typeUppercase}' AND ROUTINE_NAME='{name}'");

            return result.FirstResult;
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
                string Name = s[3];
                string Type = s[5];
                bool CanBeNull = s[10].Equals("0");
                result.Add(new ColumnDescription(Name, Type, CanBeNull, this));
            }
            return result;
        }
    }
}