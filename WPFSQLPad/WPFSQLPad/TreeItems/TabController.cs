using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WPFSQLPad.View;

namespace WPFSQLPad.ViewModel
{
    public class TabController : INotifyPropertyChanged
    {
        private ObservableCollection<TabContent> tabs;
        public ObservableCollection<TabContent> Tabs
        {
            get => tabs;
            set => Set(ref tabs, value);
        }

        private readonly Logger logger;

        public TabController(Logger logger)
        {
            this.logger = logger;
        }

        public void CloseTab(TabContent tab)
        {
            Tabs.Remove(tab);
        }

        public void CloseAllTabs()
        {
            Tabs.Clear();
            logger.WriteLine("Closed all tabs.");
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
