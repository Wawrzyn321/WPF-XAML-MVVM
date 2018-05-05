using System;
using System.Runtime.InteropServices;
using System.Security;

namespace DatabaseConnectionDialog
{
    /// <summary>
    /// Helper class for SecureString.
    /// </summary>
    public static class SecureStringUtility
    {
        //return ordinary String object
        public static string SecureStringToString(SecureString value)
        {
            if(value == null) throw new ArgumentException("Argument cannot be null!");

            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

    }
}
