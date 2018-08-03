using System;
using System.Windows;
using System.Windows.Input;
using QueryTabControl;
using WPFSQLPad.ConnectionWrappers;
using WPFSQLPad.TreeItems;
using WPFSQLPad.View;
using WPFSQLPad.ViewModel;

namespace WPFSQLPad
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        public object SelectedTreeItem => DatabasesTree.SelectedItem;

        public event Action<DatabaseConnectionWrapper> OnDatabaseChoiceRequested;
        public event Action<DatabaseBranch> OnDatabaseRefreshRequested;
        public event Action<DatabaseBranch> OnDatabaseCloseRequested;
        public event Action<DatabaseBranch> OnSetDatabaseAsCurrentRequested;
        public event Action OnCloseAllConnectionsRequested;
        public event Action<Routine> OnRoutineSourceRequested;


        public ICommand ChooseDatabaseCommand { get; private set; }
        public ICommand RefreshDatabaseConnectionCommand { get; private set; }
        public ICommand CloseDatabaseConnectionCommand { get; private set; }
        public ICommand SetConnectionAsCurrentCommand { get; private set; }
        public ICommand CopyRoutineSourceCommand { get; private set; }
        public ICommand CloseAllConnectionsCommand { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            
            DataContext = new ViewModel.ViewModel(this);

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ChooseDatabaseCommand = new ActionCommand<DatabaseConnectionWrapper>(connection => OnDatabaseChoiceRequested?.Invoke(connection));
            RefreshDatabaseConnectionCommand = new ActionCommand<DatabaseBranch>(branch => OnDatabaseRefreshRequested?.Invoke(branch));
            CloseDatabaseConnectionCommand = new ActionCommand<DatabaseBranch>(branch => OnDatabaseCloseRequested?.Invoke(branch));
            SetConnectionAsCurrentCommand = new ActionCommand<DatabaseBranch>(branch => OnSetDatabaseAsCurrentRequested?.Invoke(branch));
            CopyRoutineSourceCommand = new ActionCommand<Routine>(routine => OnRoutineSourceRequested?.Invoke(routine));
            CloseAllConnectionsCommand = new ActionCommand(() => OnCloseAllConnectionsRequested?.Invoke());
        }
    }


}
