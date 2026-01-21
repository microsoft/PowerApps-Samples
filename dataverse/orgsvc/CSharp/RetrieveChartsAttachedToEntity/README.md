# Retrieve all charts attached to a table

This sample shows how to retrieve all the organization-owned visualizations attached to a table by using the [IOrganizationService.RetrieveMultiple](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.retrievemultiple) method.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `IOrganizationService` message is intended to be used in a scenario where it contains data that provides programmatic access to the metadata and data for an organization.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

The `newSavedQuery` method creates a query for retrieving all organization-owned visualizations that are attached to the account table.


### Clean up

This sample creates no records. No cleanup is required.
