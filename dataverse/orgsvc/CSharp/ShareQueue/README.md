# Share a queue

This sample shows how to give a user or team access to a queue. The [AddPrincipalToQueueRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.addprincipaltoqueuerequest) adds the specified principal to the list of queue members. If the passed-in security principal is a team each member of the team is added to the queue.

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The [AddPrincipalToQueueRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.addprincipaltoqueuerequest) message is intended to be used in a scenario where it contains the data to add the specified principal to the list of queue members. If the principal is a team, add each team member to the queue.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org. 
1. Creates required data that this sample requires.

### Demonstrate

The `AddPrincipalToQueueRequest` message shares a queue to a team.

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.