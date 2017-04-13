# ASP.NET Core Impersonation

**This library is for the ASP.NET Core (.NET Framework 4.6+) version only, this is because its using the advapi32.dll for the impersonation.**

The library gives the ability to impersonate a user at either a function level or a middleware level. If the impersonation configuration file does not exist it will simply run the function and middleware as the underlining user. 
This can be quite useful if you want to impersonate at a local dev level but then when you push to product you want it to run as the application pool user. To achieve this the only thing, you need to do is to exclude publishing the impersonation configuration file to the product server or delete the file on it. By doing this
it helps to maintain security as you’re not pushing credentials in a file onto a product server only in your local environment.


## Impersonation Configuration File Structure
The library will look for a "impersonation.json" file by default that should be located at the root of the project, it is possible to change the name of the configuration file in the function and middleware if required.

To Get started you will need to build the configuration file "impersonation.json" in the root of the directory for your project with the following structure:-
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

Note: if the is_enabled flag is set to false it will not attempt to run the impersonation with the credentials supplied.

## Impersonation Function
The Impersonation function allows you to impersonate a different user when you run a supplied function in ASP.NET Core. 

The main purpose of the function is to help with loading possible configuration and caching functions in the Startup.cs file without having to create a middleware plugin to try and achieve this. 
using a middleware plugin to try and load a onetime configuration or cache functionality is not ideal as its only when the first request comes through the pipeline will the call be wrapped in
an impersonation level. This would also mean you need to wrap the middleware in a static flag to determine if it’s already been run once and to ignore it in all future requests.

### Getting Started
Download and reference the release DLL file in your project.

In your Startup.cs file add the following namespace.
```C#
using AspNETCore.Impersonation;
```

In your Startup.cs file add the following code below to the function 'Configure'. 
```C#
ImpersonateFunction.Run('Your Function Here', env);
```

## Middleware Plugin
The middleware plugin allows you to impersonate a different user in the ASP.NET Core pipeline. 

### Getting Started
Download and reference the release DLL file in your project.

In your Startup.cs file add the following namespace.
```C#
using AspNETCore.Impersonation;
```

In your Startup.cs file add the following code below to the function 'Configure', it must be right at the start of the function before any of the pipeline calls.
```C#
app.UseMiddleware<Impersonate>(env);
```

### Extra Options
The middleware plugin constructor looks like the following: -

```C#
public Impersonate(RequestDelegate next, IHostingEnvironment env, string filename = "impersonation.json", bool throwError = false)
```

The constructor parameters are used as followed: -

* env: this is the environment which is used on the startup.cs configure function
* filename: by default this is "impersonation.json" but you can change it to a different name if required
* throwError: if there is an exception i.e. login failure etc. this tells it to throw the exception rather than just ignore it, can be good for debugging

## Copyright and License
Copyright &copy; 2017 David Whitehead

This project is licensed under the MIT License.

You do not have to do anything special by using the MIT license and you don't have to notify anyone that your using this license. You are free to use, modify and distribute this software in any normal and commercial usage. If being used for any commercial purposes the latest copyright license file supplied above which is known as "LICENSE" must also be distributed with any compiled code that is being sold that utilises this plugin.