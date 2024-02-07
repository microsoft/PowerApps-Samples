# Add a security principal (user or team) to a queue (early bound)

This sample shows how to give a user or team access to a queue. The [AddPrincipalToQueueRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.addprincipaltoqueuerequest) adds the specified principal to the list of queue members. If the passed-in security principal is a team each member of the team is added to the queue.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The [AddPrincipalToQueueRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.addprincipaltoqueuerequest) message is intended to be used in a scenario where it contains data that is needed to add the specified principal to the list of queue members. If the principal is a team, add each team member to the queue.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `Queue` method creates a queue instance and set its property values. The returned Guids are stored in a variable.
3. The `QueryExpression` retrieves the default business unit for the creation of the team and role.
4. Creates a new example team and role required for the sample.
5. Retrieves the `prvReadQueue` and `prvAppendToQueue` privileges.
6. The `AddPrivilegeRoleRequest` method adds the `prvReadQueue` abd `prvAppendToQueue` privileges to the example role.

### Demonstrate

The `AddPrincipalToQueueRequest` method adds the team to the queue.

### Clean up

Display an option to delete the sample data in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
