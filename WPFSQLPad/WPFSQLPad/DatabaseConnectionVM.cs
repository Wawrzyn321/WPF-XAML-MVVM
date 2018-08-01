using System.ComponentModel;
using System.Runtime.CompilerServices;
using WPFSQLPad.TreeItems;

namespace WPFSQLPad
{
    public class DatabaseConnectionVM : INotifyPropertyChanged, IMenuItem
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

        private string delimiter;
        public string Delimiter
        {
            get => delimiter;
            set => Set(ref delimiter, value);
        }

        private bool isPerformingQuery;
        public bool IsPerformingQuery
        {
            get => isPerformingQuery;
            set => Set(ref isPerformingQuery, value);
        }

        #endregion

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
