using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;
namespace AspNetCore.Impersonation
{
    public partial class Impersonate
    {
        //Constructor method for initiating the middleware plugin
        //Note: Remember a middleware plugin class is only initialised once and then it just continues to run its Invoke method.
        public Impersonate(RequestDelegate next, IHostingEnvironment env, string filename = "impersonation.json", bool throwError = false)
        {
            //Passes the pipeline delegate
            this.next = next;

            //Checks if the config file exists at this point
            if (File.Exists(Path.Combine(env.ContentRootPath, filename)))
            {
                //Inits the config if it does not exists yet        
                var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile(filename, optional: false, reloadOnChange: true);
                Configuration = builder.Build();

                //Passes the exception variable
                throwException = throwError;

                //Flags that we have found the file and we are running impersonation
                shouldRunImpersonation = true;
            }            
        }
    }
}