# Sample: Enable duplicate detection and retrieve duplicates

This sample shows how to enable duplicate detection and retrieve duplicate records.

## How to run this sample

See [How to run samples](../../../How-to-run-samples.md) for information about how to run this sample.

## What this sample does

The `IsDuplicateDetectionEnabled` property is intended to be used in a scenario to enable duplicate detection rule for an organization and also for an entity.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `Account` method creates some account records to retrieve duplicates.
1. The `RetrieveDuplicateRequest` method retrieves the duplicate records. 
1. The `EnableDupkicateDetectionForOrg` class enables duplicate detection for an organization. 
1. To enable duplicate detection set `IsDuplicateDetectionEnabled = true`.
1. The `RetrieveEntityRequest` method retrieves the netity metadata. 
1. Set `IsDuplicateDetectionEnabled = true` to update the duplicate detection flag.
1. The `UpdateEntityRequest` updates the entity with duplicate detection set to `true`.

### Clean up

1. Display an option to delete the records created in the [Setup](#setup).

    The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
