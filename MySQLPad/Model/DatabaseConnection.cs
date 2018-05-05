using System;
using System.Collections.Generic;
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


        //must
        protected abstract void CreateConnection();

        //!must, ale dodaj Ping
        public abstract bool CheckAvailability();

        //!must, zamiast sqlexception dbexception
        public abstract bool OpenConnection();

        //!must, zamiast sqlexception dbexception
        public abstract bool CloseConnection(bool disableDispatcher = false);

        //must
        public abstract DatabaseBranch GetDatabaseDescription();

        //must
        public abstract List<ColumnDescription> GetTableDescription(string tableName);

        //!must, zamiana na DBReader
        public abstract ResultContainer Select(string query);

        //!must, zamiana na DBReader
        public abstract int ExecuteStatement(string query);

        //!must
        public abstract void Dispose();



        public override string ToString()
        {
            return $"DatabaseConnection to {Server} at database {Database}, user: {UserId}";
        }

    }
}