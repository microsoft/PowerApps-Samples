# Assign a chart to another user

This sample shows how to assign a user-owned visualization to another using the [AssignRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.assignrequest) message.

This sample requires an additional user that isn't available in your system. Create the required user manually in **Office 365** in order to run the sample without any errors. For this sample create a user profile **as is** shown below. 

**First Name**: Kevin<br/>
**Last Name**: Cook<br/>
**Security Role**: Sales Manager<br/>
**UserName**: kcook@yourorg.onmicrosoft.com<br/>

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The [AssignRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.assignrequest) message is intended to be used in a scenario where it contains the data that is needed to assign the specified record to a new owner (user or team) by changing the OwnerId column of the record.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateRequiredRecords` method creates a sample account and some opportunity records for the visualization.
3. The `newUserOwnedVisualization` method creates the visualization table.

### Demonstrate

The `AssignRequest` method assigns the visualization or chart to the newly created user.

### Clean up

Display an option to delete the sample data in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.