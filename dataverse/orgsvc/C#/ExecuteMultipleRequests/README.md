---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample shows how to execute multiple organization message requests by using a single web service method call, passing ExecuteMultipleRequest as a parameter in Microsoft Dataverse. [SOAP]"
---

# Execute multiple requests

This sample shows how to execute multiple organization message requests by using a single web service method call, passing [ExecuteMultipleRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.executemultiplerequest) as a parameter. Reducing the number of message requests that must be transmitted over the network results in increased message processing performance.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `ExecuteMultipleRequest` message is intended to be used in a scenario where it contains data that is needed to execute one or more messages requests as a single batch operation, and optionally return a collection of results.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.


### Demonstrate

1. The `ExecuteMultipleRequest` method creates the `ExecuteMultipleRequest` object.
1. The `ExecutingMultipleSettings` method assigns settings that define execution behavior: continue on error, return responses.
1. The `OrganizationRequestCollection` method creates an empty organization request collection.
1. The `CreateRequest` method is added for each table to the request collection.
1. The `GetCollectionOdEntitiesToUpdate` class updates the entities that are previously created.


### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
