using System;
using System.Security;
using Model;

namespace DatabaseConnectionDialog
{
    /// <summary>
    /// A contract for connection dialog.
    /// </summary>
    public interface IView
    {
        //view wants to connect
        event Action<SecureString> OnConnectButtonClicked;

        //finalize and close
        void ReturnToCaller(MySQLDatabaseConnection connection, bool setAsCurrent);
    }
}
