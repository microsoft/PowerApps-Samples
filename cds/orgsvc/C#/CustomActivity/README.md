# Create a custom activity

The following code example demonstrates how to create a custom activity using [CreateEntityRequest](https://docs.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createentityrequest?view=dynamics-general-ce-9) and [CreateAttributeRequest](https://docs.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createattributerequest?view=dynamics-general-ce-9).  

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

The `CreateEntityRequest` message and `CreateAttributeRequest` message is intended to be used in a scenario to create custom activity.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks the version of the current org.

### Demonstrate

1. Creates the custom activity entity using the `CreateEntityRequest` message.
2. Publishes the created custom activity entity.
3. Creates few attributes to the custom activity entity using `CreateAttributeRequest` message.

### Clean up

Display an option to delete the records in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
