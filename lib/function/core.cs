using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;

namespace AspNetCore.Impersonation
{
    public static partial class ImpersonateFunction
    {
        //This is the primary function which is run by the middleware pipeline for every request
        public static void Run(Action passedAction, IHostingEnvironment env, string filename = "impersonation.json", bool throwError = false)
        {
            //Checks if the config file exists at this point
            if (File.Exists(Path.Combine(env.ContentRootPath, filename)))
            {
                //Loads the config for the json file
                var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile(filename, optional: false, reloadOnChange: true);
                IConfigurationRoot Configuration = builder.Build();

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
                    bool methodStatus = LogonUser(Configuration.GetSection("impersonation:credentials:username").Value,
                                                 Configuration.GetSection("impersonation:credentials:domain").Value,
                                                 Configuration.GetSection("impersonation:credentials:password").Value,
                                                 LOGON32_LOGON_INTERACTIVE,
                                                 LOGON32_PROVIDER_DEFAULT,
                                                 out SafeAccessTokenHandle safeAccessTokenHandle);

                    //Checks if it was a successful logon
                    if (methodStatus)
                    {
                        //Runs the passed action within the impersonation layer
                        WindowsIdentity.RunImpersonated(safeAccessTokenHandle, () =>
                        {
                            passedAction();
                        });    
                    }
                    else
                    {
                        //This can be used to help debug what the problem is by flagging that you want it to throw the output
                        //Note: should not be active for production!
                        if (throwError)
                        {
                            int ret = Marshal.GetLastWin32Error();
                            throw new System.ComponentModel.Win32Exception(ret);
                        }
                    }
                }
            }
            else
            {
                //Run the passed action without the impersonation as no file has been supplied
                passedAction();
            }
        }
    }
}