# Working with Dataverse solutions

This sample demonstrates using the Dataverse solution APIs to work with solutions.

| Sample folder | Description | Build target |
|---------------|-------------|--------------|
| SolutionAttributeExport | Connects to the Organization service, creating an unmanaged solution, finding specific attributes and adding them to the solution, then exporting the managed version of the solution. | .NET 6 |

## Instructions

1. Clone the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.

1. Open the *Solutions.sln* solution file in Visual Studio 2022 located under `dataverse/orgsvc/C#-NETCore/Solutions/SolutionAttributeExport/`.

1. Edit the `appsettings.json` file in Solution Explorer. Set the connection string `Url` and `Username` parameters for your test environment.

   The environmentURL can be found in the [Power Platform admin center](https://admin.powerplatform.microsoft.com/home) and has the form `https://<environment-name>.crm.dynamics.com`.

1. Build the solution, then run the desired project.

When the sample runs, you're prompted in the default browser to select an environment user account and enter a password. To avoid repeated authentication every time you run a sample, insert a `Url` and `Password` parameter into the connection string in the `appsettings.json` file. For example:

```json
{
"ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://myorg.crm.dynamics.com;Username=someone@myorg.onmicrosoft.com;RedirectUri=http://localhost;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;LoginPrompt=Auto"
  }
}
```
