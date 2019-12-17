# Associate security role to a team 

This sample shows how to assign a security role to a team by using the [AssignRequest](https://docs.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.assignrequest?view=dynamics-general-ce-9) message. Note that this example does not take into consideration that a team or user can only be assigned a role from its business unit. The role to be assigned is the first from the collection that is returned by the RetrieveMultiple method. If that record is from a business unit that is different from the requesting team, the assignment fails.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

The [AssignRequest](https://docs.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.assignrequest?view=dynamics-general-ce-9) message is intended to be used in a scenario where it contains data that is needed to assign the specified record to a new owner (user or team) by changing the OwnerId attribute of the record.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateRequiredRecords` method creates the records required by the sample.

### Demonstrate

1. The `query` method retrieves a role from Common Data Service.
2. The `Associate` message assigns the role to a team.

### Clean up

1. Display an option to delete the sample data in [Setup](#setup).

    The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
