using Microsoft.AspNetCore.Http;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.Impersonation
{
    public partial class Impersonate
    {
        //Variable used to determine if the config file exists after the constructor
        //Note: this is done so on a production server if you exclude the publishing of the impersonation.json file then it will simply skip and run the user as the application pool
        //      but on the local dev area if you have the impersonation.json file it will run it as that user for you, thus making it easier for dev and production security.
        private bool shouldRunImpersonation = false;

        //Variable used to store the loaded config
        private IConfigurationRoot Configuration;

        //Variable used to determine if we should throw an exception on error
        //Note: general used for debugging
        private bool throwException;

        //Locks onto the windows logon DLL in .NET Framework 4.6+
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

        //Stores the pipeline delegate
        private readonly RequestDelegate next;
    }
}


