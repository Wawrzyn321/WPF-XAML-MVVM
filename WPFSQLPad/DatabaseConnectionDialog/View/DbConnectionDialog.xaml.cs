﻿using System;
using System.Security;
using System.Windows;
using Model.ConnectionModels;

namespace DatabaseConnectionDialog.View
{
    /// <summary>
    /// Interaction logic for DbConnectionDialog.xaml
    /// </summary>
    public partial class DbConnectionDialog : Window, IView
    {
        public event Action<SecureString> OnConnectButtonClicked;

        public DatabaseConnection Connection { get; private set; }
        public bool SetAsCurrent { get; private set; }
        public DbType DatabaseType { get; private set; }

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

        public void ReturnToCaller(DatabaseConnection connection, bool setAsCurrent, DbType type)
        {
            Connection = connection;
            SetAsCurrent = setAsCurrent;
            DatabaseType = type;
            DialogResult = connection != null;
            Close();
        }
    }
}
