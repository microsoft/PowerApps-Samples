# Sample: Retrieve multiple with the QueryByAttribute class

This sample shows how to use [QueryByAttribute](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.query.querybyattribute) in the [RetrieveMultiple](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.retrievemultiple) method.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `QueryByAttribute` class is intended to be used in a scenario where it contains a query that is expressed as a set of attribute and value pairs.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `Account` method creates 2 account records.

### Demonstrate

The `QueryByAttribute` method creates query using QueryByAttribute.

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.
