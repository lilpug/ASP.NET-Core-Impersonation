# ASP.NET Core Impersonation

ASP.NET Core Impersonation is a .NET library that gives the ability to impersonate a user at either a function level or a middleware level. 

**This library is for the ASP.NET Core (.NET Framework 4.6+) version only, this is due to the library utilising the advapi32.dll for the internal impersonation capability.**

<br />

## Getting Started
* Download and reference the release DLL file in your project.

* Create the "impersonation.json" file in the root of your directory from the structure mentioned in the "JSON Configuration File Structure" section

* add the following namespace
```C#
using AspNetCore.Impersonation;
```

<br />

### Impersonation Function
The Impersonation function allows you to impersonate a different user when you run a supplied function in ASP.NET Core. 

```C#
ImpersonateFunction.Run('Your Function Here', env);
```
<br />

### Middleware Plugin
The middleware plugin allows you to impersonate a different user in the ASP.NET Core pipeline. 

<br />

In your Startup.cs file add the following code to the function 'Configure'.

**Note:** It is very important that you add the code just before the app.UseMvc.

```C#
//Put this just before the app.UseMvc!
app.UseMiddleware<Impersonate>(env);

//This adds MVC into the pipeline
app.UseMvc…
```

<br />

The middleware plugin also has different parameters that can be supplied instead of using the default values:-


* **env:** this is the environment which is used in the startup.cs "Configure" function
* **filename:** by default this is "impersonation.json" but you can change it to a different name if required
* **throwError:** if there is an exception i.e. login failure etc. this tells it to throw the exception rather than just ignore it, can be useful for debugging

<br />

```C#
//Put this just before the app.UseMvc!
app.UseMiddleware<Impersonate>(env, "impersonation-config-name-here.json", true);

//This adds MVC into the pipeline
app.UseMvc…
```


<br />

## JSON Configuration File Structure
ASP.NET Core Impersonation will look for a "impersonation.json" file by default that should be located at the root of the project, it is possible to change the name of the configuration file in the function and middleware if required.

To get started you will need to build the configuration file "impersonation.json" in the root directory of your project with the following structure:-
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

**Note:** if the is_enabled flag is set to false it will not attempt to run the impersonation with the credentials supplied.

<br />


## Security

ASP.NET Core Impersonation uses a custom JSON configuration file to retrieve its information about what user it should be impersonating, If the configuration file does not exist it will simply run the functions and middleware as the application pool user for that project. 

This means by excluding the impersonation configuration file when you push to the production environment not only will it be running as the application pool user instead of the impersonation user but it also won’t be pushing up the impersonation credentials to the production server.

The following code shows you what the exclude line should look like in your “.csproj” file.
```
<ItemGroup>
    <Content Remove="impersonation.json" />
</ItemGroup>
```

<br />

## Problems & Solutions

### FileLoadException

If you’re seeing "FileLoadException: could not load file or assembly .... access denied" then you might need to add permission for the user your impersonating against to your project folder and all its sub directories and files.

<br />

### User AppData

If you’re seeing any kind of access denied due to the AppData folder on Windows make sure your using the middleware plugin just before the “app.UseMvc” as libraries such as the app.UseSession actually use physical files in the pipeline and you will be impersonating as a different user that might not have access to that area.

<br />

## Copyright and License
Copyright &copy; 2017 David Whitehead

This project is licensed under the MIT License.

You do not have to do anything special by using the MIT license and you don't have to notify anyone that your using this license. You are free to use, modify and distribute this software in any normal and commercial usage. If being used for any commercial purposes the latest copyright license file supplied above which is known as "LICENSE" must also be distributed with any projects using the ASP.NET Core Impersonation library