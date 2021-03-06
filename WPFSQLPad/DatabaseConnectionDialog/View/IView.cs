﻿using System;
using System.Security;
using Model.ConnectionModels;

namespace DatabaseConnectionDialog.View
{
    /// <summary>
    /// A contract for connection dialog.
    /// </summary>
    public interface IView
    {
        //view wants to connect
        event Action<SecureString> OnConnectButtonClicked;

        //finalize and close
        void ReturnToCaller(DatabaseConnection connection, bool setAsCurrent, DbType type);
    }
}
