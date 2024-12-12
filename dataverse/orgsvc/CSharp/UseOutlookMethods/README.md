# Use Dynamics 365 for Outlook methods

This sample shows how to use the methods available in the [Microsoft.Crm.Outlook.Sdk.dll](https://learn.microsoft.com/dotnet/api/microsoft.crm.outlook.sdk?view=dynamics-outlookclient-ce-9) assembly. 

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `Microsoft.Crm.Outlook.sdk` assembly is used in a scenario where it contains types that provide programmatic interaction with Microsoft Dynamics 365 for Outlook and Microsoft Dynamics 365 for Microsoft Office Outlook with Offline Access.

## How this sample works

In order to simulate the scenario described above, the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `CrmOutlookService` method sets up the service.
2. The `CrmOutlookService.IsCrmClientOffline` method checks if the client is offline.
3. The `CrmOutlookService.GoOnline()` method takes the client to online. This method will automatically sync up with database, there is no need to call the `Sync()` method.

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.