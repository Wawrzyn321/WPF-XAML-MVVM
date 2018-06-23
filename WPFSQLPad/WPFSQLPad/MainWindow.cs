﻿using System;
using System.Windows;
using System.Windows.Input;
using Model;
using Model.ConnectionModels;
using Model.TreeItems;
using WPFSQLPad.View;

namespace WPFSQLPad
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        public object SelectedTreeItem => DatabasesTree.SelectedItem;

        public event Action<TabContent> OnCloseTabRequested;
        public event Action<TabContent> OnExportTabXMLRequested;
        public event Action<TabContent> OnExportTabCSVRequested;
        public event Action<DatabaseConnection> OnDatabaseChoiceRequested;
        public event Action OnCloseAllTabsRequested;
        public event Action<DatabaseBranch> OnDatabaseRefreshRequested;
        public event Action<DatabaseBranch> OnDatabaseCloseRequested;
        public event Action<Routine> OnRoutineSourceRequested;

        public ICommand ChooseDatabaseCommand { get; private set; }
        public ICommand CloseTabCommand { get; private set; }
        public ICommand ExportTabXMLCommand { get; private set; }
        public ICommand ExportTabCSVCommand { get; private set; }
        public ICommand CloseAllTabsCommand { get; private set; }
        public ICommand RefreshDatabaseConnectionCommand { get; private set; }
        public ICommand CloseDatabaseConnectionCommand { get; private set; }
        public ICommand CopyRoutineSourceCommand { get; private set; }

        private readonly ViewModel.ViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new ViewModel.ViewModel(this);
            DataContext = viewModel;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ChooseDatabaseCommand = new ActionCommand<DatabaseConnection>(connection => OnDatabaseChoiceRequested?.Invoke(connection));
            CloseTabCommand = new ActionCommand<TabContent>(content => OnCloseTabRequested?.Invoke(content));
            ExportTabXMLCommand = new ActionCommand<TabContent>(content => OnExportTabXMLRequested?.Invoke(content));
            ExportTabCSVCommand = new ActionCommand<TabContent>(content => OnExportTabCSVRequested?.Invoke(content));
            CloseAllTabsCommand = new ActionCommand(() => OnCloseAllTabsRequested?.Invoke());
            RefreshDatabaseConnectionCommand = new ActionCommand<DatabaseBranch>(branch => OnDatabaseRefreshRequested?.Invoke(branch));
            CloseDatabaseConnectionCommand = new ActionCommand<DatabaseBranch>(branch => OnDatabaseCloseRequested?.Invoke(branch));
            CopyRoutineSourceCommand = new ActionCommand<Routine>(routine => OnRoutineSourceRequested?.Invoke(routine));
        }
    }


}
