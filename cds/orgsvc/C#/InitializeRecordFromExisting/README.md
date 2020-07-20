# Sample: Initialize a record from existing record

This sample shows how to use the [IOrganizationService.InitializeFromRequest](https://docs.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializefromrequest?view=dynamics-general-ce-9) message to create new records from an existing record.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

The `IOrganizationService.InitializeFromRequest` message is intended to be used in a scenario where it contains the data that is needed to initialize a new record from an existing record.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateRequiredRecords` method creates any entity records that this sample requires.


### Demonstrate

1. The `InitializeFromRequest` method creates the request and set properties for the request object. 
2. The `InitializeFromResponse`  method executes the request.


### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.

