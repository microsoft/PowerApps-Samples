# Execute multiple requests in transaction

This sample shows how to use a single web method call to execute all message requests in a collection as part of a single database transaction. It is a common requirement in business applications to coordinate changes of multiple records in the system so that either all the data changes succeed, or none of them do. In database terms, this is known as executing multiple operations in a single transaction with the ability to roll back all data changes should any one operation fail.

You can execute two or more organization service requests in a single database transaction using the [ExecuteTransactionRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.executetransactionrequest) message request. 


## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `ExecuteTransactionRequest` message is intended to be used in a scenario where it contains  data that is needed to execute one or more message requests in a single database transaction, and optionally return a collection of results.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `ExecuteTransactionRequest` method creates the `ExecuteTransactionRequest` object.
2. The `OrganizationRequestCollection` method creates an empty organization request collection.
3. The `CreateRequest` method is added for each table to the request collection.

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
