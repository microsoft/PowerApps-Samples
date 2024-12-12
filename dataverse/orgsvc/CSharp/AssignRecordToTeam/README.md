# Assign a record to a team

This sample shows how to assign a record to a team by using the [IOrganizationService.Update](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.update) message.

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The [IOrganizationService.Update](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.update) message is intended to be used in a scenario where it contains the data that is needed to assign the specified record to a new owner (user or team) by changing the OwnerId attribute of the record.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current  version of the org. 
1. Creates required data that this sample requires.

### Demonstrate

The `Update` message assigns the account record created for the sample to a team. 

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.
