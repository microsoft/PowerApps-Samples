# Create, retrieve, update, and delete a dashboard

This sample shows how to create, retrieve, update, and delete an user-owned dashboard. As part of updating the dashboard, it’s set to be the default dashboard for the organization. This sample uses the following common methods:

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

Checks for the current version of the org.

### Demonstrate

1. The `mySavedQuery` method grabs the default public view for opportunities. 
2. The `visualizationQuery` method retrieves the visualizations out of the system. This sample assumes that you have the **Top opportunities**. 
3. The `dashboard` method sets the dashboard and specifies the FormXml.
4. The `chartPicker` method enables the chart picker on the chart.

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.