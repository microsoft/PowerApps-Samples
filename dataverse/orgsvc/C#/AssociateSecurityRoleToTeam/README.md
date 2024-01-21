# Associate security role to a team 

This sample shows how to associate a security role to a team by using the [AssociateRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.associaterequest) message. Note that this example does not take into consideration that a team or user can only be associated to a role from its business unit. The role to be associated is the first from the collection that is returned by the `RetrieveMultiple` method. If that record is from a business unit that is different from the requesting team, the association fails.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The [AssociateRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.associaterequest) message is intended to be used in a scenario where it contains data that is needed to create a link between records.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateRequiredRecords` method creates the records required by the sample.

### Demonstrate

1. The `query` method retrieves a role from Microsoft Dataverse.
2. The `Associate` message assigns the role to a team.

### Clean up

Display an option to delete the sample data in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
