## ASP.Net Core Impersonation Middleware Plugin
the middleware plugin always users to impersonate a different user in the ASP.Net pipeline. 

It is for the ASP.Net (.Net Framework) version only.

## Getting Started
Download and reference the release DLL file in your project.

In your Startup.cs file add the following namespace.
```C#
using AspNetCore.Impersonation;
```

In your Startup.cs file add the following code below to the function Configure, it must be right at the start of the function before any of the pipeline calls.
```C#
app.UseMiddleware<Impersonate>(env);
```

The plugin will look for a "impersonation.json" file by default that should be located at the root of the project.
build the configuration file "impersonation.json" in the root of the directory with the following structure:-
```json
{
  "impersonation": {
    "is_enabled": false,
    "credentials": {
      "domain": "",
      "username": "",
      "password": ""
    }
  }
}
```

now simply change the is_enabled flag to true and put your credentials you want to impersonate on ASP.Net Core.

### Extra Options
The impersonation plugin constructor looks like the following:-

```C#
public Impersonate(RequestDelegate next, IHostingEnvironment env, string filename = "impersonation.json", bool createNoneExistStructure = false, bool throwError = false)
```

The constructor parameters are used as followed:-

* env: this is the enviroment which is used on the startup.cs configure function
* filename: by default this is "impersonation.json" but you can change it to a different name if required
* createNoneExistStructure: This creates the json file at the root of the project if it does not exist, by default it does not do it
* throwError: if there is an exception i.e. login failure etc this tells it to throw the exception rather than just ignore it, can be good for debugging

## Copyright and License
Copyright &copy; 2017 David Whitehead

This project is licensed under the MIT License.

You do not have to do anything special by using the MIT license and you don't have to notify anyone that your using this license. You are free to use, modify and distribute this software in any normal and commercial usage. If being used for any commercial purposes the latest copyright license file supplied above which is known as "LICENSE" must also be distributed with any compiled code that is being sold that utilises this plugin.