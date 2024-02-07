# Release a queue item to the queue (early bound)

This sample shows how to use [ReleaseToQueueRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.releasetoqueuerequest) to dissociate a user from a queue item that he or she worked on and release a queue item back to the queue.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `ReleaseToQueueRequest` message is intended to be used in a scenario where it contains data that is needed to assign a queue item back to the queue owner so others can pick it.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `Queue` message creates a new queue and store its returned GUIDs in a variable.
3. The `QueueItem` message creates a new instance of a queueitem and initialize its properties.
4. The `WhoAMIRequest` retrieves the current user's information.

### Demonstrate

The `ReleaseToQueueRequest` message removes worker from queue item to release queued object from worker's queue.

### Clean up

Display an option to delete the sample data created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
