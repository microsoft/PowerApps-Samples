# Sample: Use FetchXML with a paging cookie

This sample shows how to use the paging cookie in a FetchXML query to retrieve successive pages of query results. It uses the [IOrganizationService. RetrieveMultiple](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.retrievemultiple) method.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `IOrganizationService.Retrieve` method is intended to be used in a scenario where it contains data that retrieves all the records.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `Account` method creates an parent account record and also 10 child account records.

### Demonstrate

1. The `fetchXml` creates the FetchXML string for retrieving all child accounts to a parent account. This fetch query is using 1 placeholder to specify the parent account id for filtering out required accounts.

### Clean up

1. Displays an option to delete all the data created in the sample.

The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.

