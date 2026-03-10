# Getting started with the Dataverse ServiceClient

This collection of samples demonstrates authoring code for Microsoft Dataverse using the newly released [Microsoft.PowerPlatform.Dataverse.Client](https://www.nuget.org/packages/Microsoft.PowerPlatform.Dataverse.Client/) NuGet package. Specifically, using the [ServiceClient](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient) class to authenticate the logged on user, connect to the web service, and execute message requests.

More information: [Transition apps to Dataverse ServiceClient](https://learn.microsoft.com/power-apps/developer/data-platform/sdk-client-transition)

The provided code samples are listed below.

|Sample folder|Description|Build target|
|---|---|---|
|WhoAmI|Demonstrates connecting to the Organization service and executing a simple message (WhoAmI).|.NET 6|
|CreateUpdateDelete|Demonstrates connecting to the Organization service and executing Create, Update, Retrieve, and Delete data operations.|.NET 6|
|TelemetryUsingILogger|Demonstrates connecting to the Organization service and executing a simple message. In addition, processing by the web service is logged through the `ILogger` interface to the console.|.NET 6|

## Instructions

1. Clone the PowerApps-Samples repository.

1. Locate the /dataverse/orgsvc/CSharp-NETx/ServiceClient/ folder.

1. Open the *ServiceClient.sln* solution file in Visual Studio 2022.

1. Edit the *appsettings.json* file in the **Solution Items** folder of Solution Explorer. Set the connection string `Url` and `Username` parameters as appropriate for your test environment.

 The environment Url can be found in the Power Platform admin center. It has the form https://\<environment-name>.crm.dynamics.com.

1. Build the solution, and then run the desired project.

When each sample runs, you will be prompted in the default browser to select an environment user account and enter a password. To avoid having to do this every time you run a sample, insert a password parameter into the connection string in the appsettings.json file. For example:

```json
{
"ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://myorg.crm.dynamics.com;Username=someone@myorg.onmicrosoft.com;Password=mypassword;RedirectUri=http://localhost;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;LoginPrompt=Auto"
  }
}
```

**Tip**: You can set a user environment variable named DATAVERSE_APPSETTINGS to the file path of the appsettings.json file stored anywhere on your computer. The samples will use that appsettings file if the environment variable exists and is not null. Be sure to log out and back in again after you define the variable for it to take affect. To set an environment variable, go to **Settings > System > About**, select **Advanced system settings**, and then choose **Environment variables**.
