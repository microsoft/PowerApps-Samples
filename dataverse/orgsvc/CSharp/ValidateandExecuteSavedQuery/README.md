# Sample: Validate and execute a saved query

This sample shows how to use the [IOrganizationService.ValidateSavedQueryRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.validatesavedqueryrequest) message to validate a FetchXML query, and then use the [IOrganizationService.ExecuteByIdSavedQueryRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.executebyidsavedqueryrequest) message to execute the query.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `ValidateSavedQueryRequest` class is intended to be used in a scenario where it contains the data that is needed to validate a saved query (view). 

The `ExecuteByIdSavedQueryRequest` class is intended to be used in a scenario where it contains data that is needed to execute a saved query (view) that has the specified ID.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `Account` method creates 3 account records.
1. The `SavedQuery` method creates a Saved query.
1. The `UserQuery` method creates a User query.

### Demonstrate

1. The `ValidateSavedQueryRequest` method creates the validate request.
1. The `ExecuteByIdSavedQueryRequest` method executes the saved query.

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.
