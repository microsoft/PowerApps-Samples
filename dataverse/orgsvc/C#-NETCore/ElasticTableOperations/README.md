# SDK for .NET Elastic Table Operations Sample

This .NET 6.0 sample demonstrates how to perform operations with file columns using the Dataverse SDK for .NET.

This sample uses the [Microsoft.PowerPlatform.Dataverse.Client.ServiceClient Class](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient).

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with system administrator or system customizer privileges.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the [PowerApps-Samples/dataverse/orgsvc/C#-NETCore/FileOperations/FileOperations.sln](FileOperations.sln) file using Visual Studio 2022.
1. Edit the *appsettings.json* file. Set the connection string `Url` and `Username` parameters as appropriate for your test environment.

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

>**Tip**: You can set a user environment variable named DATAVERSE_APPSETTINGS to the file path of the appsettings.json file stored anywhere on your computer. The samples will use that appsettings file if the environment variable exists and is not null. Be sure to log out and back in again after you define the variable for it to take affect. To set an environment variable, go to **Settings > System > About**, select **Advanced system settings**, and then choose **Environment variables**.

## Sample Output

The output of the sample should look something like this:

```
TODO
```

## Demonstrates

The project uses a `Utility` class to perform operations involving creating or retrieving schema data.

The code for this sample is in the [Program.cs](Program.cs) file.

This project performs these operations:

### Create an elastic table named 'contoso_SensorData'

The `Utility.CreateSensorDataTable` 

### Next


### Clean up

To leave the system in the state before the sample ran, it does the following:

- Delete the account record
- Delete the file column