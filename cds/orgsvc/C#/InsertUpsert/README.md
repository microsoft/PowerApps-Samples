# Assign a security role to a team

This sample shows how to assign a security role to a team by using the [AssignRequest](https://docs.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.assignrequest?view=dynamics-general-ce-9) message.

## How to run this sample

See [How to run samples](../../../README.md) for information about how to run this sample.

## What this sample does

The [AssignRequest](https://docs.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.assignrequest?view=dynamics-general-ce-9) message is intended to be used in a scenario where it contains data that is needed to assign specific user to a team. This sample does not take into consideration that a team or user can only be assigned a role from its business unit. The role to be assigned is the first from the collection that is returned by the `RetrieveMultiple` method. If that record is from a business unit that is different from the requesting team, the assignment fails.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateRequiredRecords` method creates sample entity records required for the sample.
3. The `query` method retrieves the default business unit needed to create the team and role.
4. The `setupTeam` method instantiates a team entity record and set its property values.
5. The `setupRole`method instantiates a role entity record and set its property value.
6. The `queryPrivileges` method queries the privileges that we want to add to the role.
7. The `AddPrivilegesRoleRequest` method adds the retrieved privileges to the example role.

### Demonstrate

1. The `QueryExpression` method retrieves the role from Common Data service.
2. The `_service.Associate` expression adds the retrieved role to the team.

### Clean up

1. Display an option to delete the sample data in [Setup](#setup).

   The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.