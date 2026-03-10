# Sample: Enable duplicate detection and retrieve duplicates

This sample shows how to enable duplicate detection and retrieve duplicate records.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `IsDuplicateDetectionEnabled` property is intended to be used in a scenario to enable duplicate detection rule for an organization and also for a table.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `Account` method creates some account records to retrieve duplicates.
1. The `RetrieveDuplicateRequest` method retrieves the duplicate records. 
1. The `EnableDuplicateDetectionForOrg` class enables duplicate detection for an organization. 
1. To enable duplicate detection set `IsDuplicateDetectionEnabled = true`.
1. The `RetrieveEntityRequest` method retrieves the table definitions. 
1. Set `IsDuplicateDetectionEnabled = true` to update the duplicate detection flag.
1. The `UpdateEntityRequest` updates the table with duplicate detection set to `true`.

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
