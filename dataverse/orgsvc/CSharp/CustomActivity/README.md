# Create a custom activity

The following code example demonstrates how to create a custom activity using [CreateEntityRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createentityrequest) and [CreateAttributeRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createattributerequest).  

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `CreateEntityRequest` message and `CreateAttributeRequest` message is intended to be used in a scenario to create custom activity.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks the version of the current org.

### Demonstrate

1. Creates the custom activity table using the `CreateEntityRequest` message.
2. Publishes the created custom activity table.
3. Creates few columns to the custom activity table using `CreateAttributeRequest` message.

### Clean up

Display an option to delete the records in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
