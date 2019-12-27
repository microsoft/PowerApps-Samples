# Add record to a queue

This sample shows how to add a record to a queue. 

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

This sample creates source and destination queues. It adds a letter activity to the source queue and then moves it to the destination queue.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current  version of the org. 
1. Creates required data that this sample requires.

### Demonstrate

1. The `RetrieveUserQueueRequest` message gets the known private queues for the user.
1. The `AddToQueueRequest` message moves the record from source queue to destination queue. 

### Clean up

1. Displays an option to delete all the data created in the sample.

The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.