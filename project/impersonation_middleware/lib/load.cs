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
        public Impersonate(RequestDelegate next, IHostingEnvironment env, string filename = "impersonation.json", bool createNoneExistStructure = false, bool throwError = false)
        {
            //Passes the pipeline delegate
            this.next = next;

            //If the flag is true it checks if the correct structure exists and if not it creates it
            CreateStructureCheck(env, filename, createNoneExistStructure);

            //Inits the config if it does not exists yet        
            var builder = new ConfigurationBuilder()
           .SetBasePath(env.ContentRootPath)
           .AddJsonFile(filename, optional: false, reloadOnChange: true);
            Configuration = builder.Build();

            //Passes the exception variable
            throwException = throwError;
        }

        //This function checks if the structure exists and if not it creates the generic template of the impersonation file
        private void CreateStructureCheck(IHostingEnvironment env, string filename, bool create)
        {
            if (create)
            {
                //If the json file does not exist, it creates the structure automatically
                if (!File.Exists(Path.Combine(env.ContentRootPath, filename)))
                {
                    string jsonStructure =
    $@"{{
  ""impersonation"": {{
    ""is_enabled"": false,
    ""credentials"": {{
      ""domain"": """",
      ""username"": """",
      ""password"": """"
    }}
  }}
}}";
                    //Creates the json file in the base directory as it does not already exist
                    File.WriteAllText(Path.Combine(env.ContentRootPath, filename), jsonStructure);
                }
            }
        }
    }
}