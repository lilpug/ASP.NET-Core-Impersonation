using Microsoft.AspNetCore.Http;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.Impersonation
{
    public partial class Impersonate
    {
        //Variable used to store the loaded config
        private IConfigurationRoot Configuration;

        //Variable used to determine if we should throw an exception on error
        //Note: general used for debugging
        private bool throwException;

        //Locks onto the windows logon DLL in .NET Framework 4.6+
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

        //Stores the pipeline delegate
        private readonly RequestDelegate next;
    }
}


