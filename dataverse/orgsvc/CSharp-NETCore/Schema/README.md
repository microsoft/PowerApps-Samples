# Schema samples

This collection of samples demonstrates message request and response with Dataverse schema definitions, using the [Microsoft.PowerPlatform.Dataverse.Client](https://www.nuget.org/packages/Microsoft.PowerPlatform.Dataverse.Client/) NuGet package with the [ServiceClient](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient) class.

Learn more in [Transition apps to Dataverse ServiceClient](https://learn.microsoft.com/power-apps/developer/data-platform/sdk-client-transition).

| Sample folder | Description | Build target |
|---------------|-------------|--------------|
| [RetrieveMetadataChanges](RetrieveMetadataChanges) | Demonstrates the use of the [RetrieveMetadataChangesRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrievemetadatachangesrequest) and [RetrieveMetadataChangesResponse](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrievemetadatachangesresponse) classes to create a simple client-side persistent cache of Dataverse schema data. | .NET 6 |

## Instructions

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.

1. Locate the `/dataverse/orgsvc/C#-NETx/Schema/` folder.
1. Open the `Schema.sln` solution file in Visual Studio 2022.
1. Edit the `appsettings.json` file to set the connection string `Url` and `Username` parameters for your test environment.

   For example:

   ```json
   {
      "ConnectionStrings": {
         "default": "AuthType=OAuth;Url=https://myorg.crm.dynamics.com;Username=someone@myorg.onmicrosoft.com;RedirectUri=http://localhost;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;LoginPrompt=Auto"
      },
   }
   ```

   Your environment URL can be found in the [Power Platform admin center](https://admin.powerplatform.microsoft.com/) and has the form `https://<environment-name>.crm.dynamics.com`.
1. Build the solution, then run the project.

> [!TIP]
> You can set a user environment variable named `DATAVERSE_APPSETTINGS` to the file path of the `appsettings.json` file stored anywhere on your computer. The samples uses that file if the environment variable exists and isn't null. Log out and back in again after you define the variable for it to take affect. You can manually set an environment variable by typing `env` in your system search bar, then choose **Environment variables** in **System properties** window.
