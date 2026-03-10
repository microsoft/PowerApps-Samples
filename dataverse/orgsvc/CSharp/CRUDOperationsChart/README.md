# Create, retrieve, update, and delete a chart

This sample shows how to create, retrieve, update, and delete an user-owned visualization using the following methods:

- [IOrganizationService.Create](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.create)
- [IOrganizationService.Retrieve](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.retrieve)
- [IOrganizationService.Update](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.update)
- [IOrganizationService.Delete](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.delete)

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `IOrganizationService` message is intended to be used in a scenario where it contains data that provides programmatic access to the metadata and data for an organization.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `CreateRequiredRecords` method creates table records that is required for the sample.

### Demonstrate

1. The `presentationXml` method sets the presentation XML string. 
2. The `dataXml` method sets the data XML string.
3. The `newUserOwnedVisualization` method creates the visualization table.
4. The `retrievedOrgOwnedVisualization` method retrieves the visualization.
5. The `newDataXml` method updates the name and the data description string.

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.