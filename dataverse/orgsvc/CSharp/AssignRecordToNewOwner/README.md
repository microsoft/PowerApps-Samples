# Assign a record to a new owner

This sample shows how to assign an account to another user by using the [IOrganizationService.Update](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.update) message.

This sample uses the `IOrganization.Update` method rather than the [AssignRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.assignrequest) because there is an effort to remove specialized messages. More information: [Perform specialized operations using update](https://learn.microsoft.com/powerapps/developer/common-data-service/special-update-operation-behavior)

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The [IOrganizationService.Update](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.update) message is intended to be used in a scenario where it contains the data that is needed to update existing record.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org. 
1. Creates required data that this sample requires.

### Demonstrate

1. The `Retrieve` method retrieves the account records created in the setup(#setup).
1. The `Update` message updates the `ownerid` attribute to the user who you want to own the record. 

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.