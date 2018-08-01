using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Model.ConnectionModels;
using WPFSQLPad.TreeItems;

namespace WPFSQLPad.ConnectionWrappers
{
    public abstract class DatabaseConnectionWrapper : INotifyPropertyChanged, IMenuItem
    {

        #region Observed Properties

        #region IMenuItem Members

        protected bool isChoosen;
        public bool IsChoosen
        {
            get => isChoosen;
            set => Set(ref isChoosen, value);
        }

        public bool IsPlaceholder => false;

        public string Description
        {
            get => connectionReference.Description;
            set => throw new InvalidOperationException("DatabaseConnectionWrapper: cannot change database description!");
        }

        #endregion

        protected bool isAvailable;
        public bool IsAvailable
        {
            get => isAvailable;
            set => Set(ref isAvailable, value);
        }

        public DbType DatabaseType => connectionReference.DatabaseType;
        public string Delimiter => connectionReference.Delimiter;
        public bool IsPerformingQuery => connectionReference.IsPerformingQuery;

        #endregion

        public readonly DatabaseConnection connectionReference;
        protected readonly ExternalTimeDispatcher connectionCheck;
        protected readonly DatabaseDataExtractor dataExtrator;

        protected DatabaseConnectionWrapper(DatabaseConnection connectionReference, DatabaseDataExtractor dataExtractor)
        {
            this.connectionReference = connectionReference;
        }

        public abstract List<ColumnDescription> GetTableDescription(string tableName);

        public abstract DatabaseBranch GetDatabaseDescription();

        public abstract List<Routine> GetRoutines();

        public abstract string GetRoutineCode(Routine.RoutineType type, string name);

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(ref T oldValue, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(oldValue, value))
            {
                return false;
            }
            else
            {
                oldValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
        }

        #endregion

    }
}
