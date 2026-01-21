# Retrieve the roles for an organization

This sample shows how to retrieve the roles for an organization by using the [IOrganizationService.RetrieveMultiple](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.retrievemultiple) method.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The [IOrganizationService.RetrieveMultiple](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.retrievemultiple) message is intended to be used in a scenario where it contains data  that is needed to retrieve a collection of records.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

The `query` method retrieves all the roles that are present in an organization.

### Clean up

This sample creates no records. No cleanup is required.