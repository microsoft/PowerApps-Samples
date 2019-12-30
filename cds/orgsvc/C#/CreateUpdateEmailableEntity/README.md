# Create an Email entity

This sample shows how to create and update an entity using the [CreateEntityRequest](https://docs.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.createentityrequest?view=dynamics-general-ce-9) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

The `CreateEntityRequest` message is intended to be used in a scenario where it contains  the data that is needed to create a custom entity, and optionally, to add it to a specified unmanaged solution.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `Entity` method creates the custom entity. Define the entity to enable for emailing. In order to do so, `IsActivityParty` must be set to true.
2. The `StringAttributeMetadata` method is used to define the primary attribute of the entity which is used in the ActivityParty screens. Be sure to select descriptive attributes.
3. The `PublishRequest` method publishes all the customizations.
4. The `CreateFirstEmailAttributeRequest` method creates an email attribute in order to create emails from the entity.

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.

