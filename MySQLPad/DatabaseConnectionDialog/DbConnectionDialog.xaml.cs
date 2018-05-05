using System;
using System.Security;
using System.Windows;
using Model;

namespace DatabaseConnectionDialog
{
    /// <summary>
    /// Interaction logic for DbConnectionDialog.xaml
    /// </summary>
    public partial class DbConnectionDialog : Window, IView
    {
        public event Action<SecureString> OnConnectButtonClicked;

        public MySQLDatabaseConnection Connection { get; private set; }
        public bool SetAsCurrent { get; private set; }

        public DbConnectionDialog()
        {
            InitializeComponent();

            ViewModel viewModel = new ViewModel(this);
            DataContext = viewModel;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            SecureString password = passwdBox.SecurePassword;
            OnConnectButtonClicked?.Invoke(password);
        }

        public void ReturnToCaller(MySQLDatabaseConnection connection, bool setAsCurrent)
        {
            Connection = connection;
            SetAsCurrent = setAsCurrent;
            DialogResult = connection != null;
            Close();
        }
    }
}
