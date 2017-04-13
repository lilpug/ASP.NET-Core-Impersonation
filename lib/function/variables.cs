using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace AspNetCore.Impersonation
{
    public static partial class ImpersonateFunction
    {
        //Locks onto the windows logon DLL in .NET Framework 4.6+
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);
    }
}