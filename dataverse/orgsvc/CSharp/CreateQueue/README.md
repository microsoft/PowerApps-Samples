# Create a queue

This sample shows how to create a simple queue and set the required attributes using the [IOrganizationService.Create](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.create) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `IOrganizationService` message is intended to be used in a scenario where it contains data that provides programmatic access to the metadata and data for an organization.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `newQueue` method creates a queue instance and set its property values. 
2. The `IncomingEmailDeliveryMethods` defines the anonymous types to define the range of possible queue property values.

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.

