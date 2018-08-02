﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPFSQLPad.ViewModel
{
   public class ConnectionContainer : INotifyPropertyChanged
    {
        private readonly Logger logger;

        public ConnectionContainer(Logger logger)
        {
            this.logger = logger;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T oldValue, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(oldValue, value))
            {
                return false;
            }
            else
            {
                oldValue = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        #endregion
    }
}