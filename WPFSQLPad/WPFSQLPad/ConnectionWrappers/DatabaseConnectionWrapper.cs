using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Model.ConnectionModels;
using WPFSQLPad.ConnectionWrappers.DataExtractors;
using WPFSQLPad.IMenuItems;
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
            get => ConnectionReference.Description;
            set => throw new InvalidOperationException("DatabaseConnectionWrapper: cannot change database description!");
        }

        #endregion

        protected bool isAvailable;
        public bool IsAvailable
        {
            get => isAvailable;
            set => Set(ref isAvailable, value);
        }

        public DbType DatabaseType => ConnectionReference.DatabaseType;
        public string Delimiter => ConnectionReference.Delimiter;
        public bool IsPerformingQuery => ConnectionReference.IsPerformingQuery;

        #endregion

        public readonly DatabaseConnection ConnectionReference;
        protected readonly ExternalTimeDispatcher connectionCheck;
        protected DatabaseDataExtractor dataExtrator;

        protected DatabaseConnectionWrapper(DatabaseConnection connectionReference)
        {
            this.ConnectionReference = connectionReference;
        }

        public bool CloseConnection()
        {
            return ConnectionReference.CloseConnection(true);
        }

        public abstract DatabaseBranch GetDatabaseDescription();

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
