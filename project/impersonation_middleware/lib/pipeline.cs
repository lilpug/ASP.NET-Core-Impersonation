using System.Threading.Tasks;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace AspNetCore.Impersonation
{
    public partial class Impersonate
    {
        //This is the primary function which is run by the middleware pipeline for every request
        public async Task Invoke(HttpContext context)
        {
            bool methodStatus = false;

            //Checks if the value is a valid boolean and if its true, otherwise just passes the call on and does not wrap it in impersonation
            if (
                //Checks the value is a valid boolean and that its true
                bool.TryParse(Configuration.GetSection("impersonation:is_enabled").Value, out bool isEnabled) && isEnabled &&

                //Checks the credentials are not empty
                Configuration.GetSection("impersonation:credentials:domain") != null && !string.IsNullOrWhiteSpace(Configuration.GetSection("impersonation:credentials:domain").Value) &&
                Configuration.GetSection("impersonation:credentials:username") != null && !string.IsNullOrWhiteSpace(Configuration.GetSection("impersonation:credentials:username").Value) &&
                Configuration.GetSection("impersonation:credentials:password") != null && !string.IsNullOrWhiteSpace(Configuration.GetSection("impersonation:credentials:password").Value)
              )
            {
                const int LOGON32_PROVIDER_DEFAULT = 0;

                //This parameter causes LogonUser to create a primary token. 
                const int LOGON32_LOGON_INTERACTIVE = 2;

                // Call LogonUser to obtain a handle to an access token. 
                methodStatus = LogonUser(Configuration.GetSection("impersonation:credentials:username").Value,
                                             Configuration.GetSection("impersonation:credentials:domain").Value,
                                             Configuration.GetSection("impersonation:credentials:password").Value,
                                             LOGON32_LOGON_INTERACTIVE,
                                             LOGON32_PROVIDER_DEFAULT,
                                             out SafeAccessTokenHandle safeAccessTokenHandle);

                //Checks if it was a successful logon
                if (methodStatus)
                {
                    await WindowsIdentity.RunImpersonated(safeAccessTokenHandle, async () =>
                    {
                        await next.Invoke(context);
                    });
                }
                else
                {
                    //This can be used to help debug what the problem is by flagging that you want it to throw the output
                    //Note: should not be active for production!
                    if (throwException)
                    {
                        int ret = Marshal.GetLastWin32Error();
                        throw new System.ComponentModel.Win32Exception(ret);
                    }
                }
            }

            //Checks if the status is false and if so then just forwards the call on without any identity impersonation
            if (!methodStatus)
            {
                //Just forwards the call without changing the user identity
                await next.Invoke(context);
            }
        }
    }
}