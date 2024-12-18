# Working with Dataverse solutions

This sample demonstrates using the Dataverse solution APIs to work with solutions.

The provided code samples are listed below.

|Sample folder|Description|Build target|
|---|---|---|
|SolutionAttributeExport|Demonstrates connecting to the Organization service, creating an unmanaged solution, finding specific attributes and adding them to the solution, then exporting the managed version of the solution.|.NET 6|

## Instructions

1. Clone the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.

1. Open the *Solutions.sln* solution file in Visual Studio 2022 located under dataverse/orgsvc/CSharp-NETCore/Solutions/SolutionAttributeExport/.

1. Edit the *appsettings.json* file in the **Solution Items** folder of Solution Explorer. Set the connection string `Url` and `Username` parameters as appropriate for your test environment.

 The environment Url can be found in the Power Platform admin center. It has the form https://\<environment-name>.crm.dynamics.com.

1. Build the solution, and then run the desired project.

When the sample runs, you will be prompted in the default browser to select an environment user account and enter a password. To avoid having to do this every time you run a sample, insert a password parameter into the connection string in the appsettings.json file. For example:

```json
{
"ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://myorg.crm.dynamics.com;Username=someone@myorg.onmicrosoft.com;Password=mypassword;RedirectUri=http://localhost;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;LoginPrompt=Auto"
  }
}
```
