# Dump global option set information to a file

This sample shows how to dump global option set information to a file. It uses the [RetrieveAllOptionSetsRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrievealloptionsetsrequest) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `RetrieveAllOptionSetsRequest` message is intended to be used in a scenario that contains data that is needed o retrieve information about all global option sets.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `RetrieveAllOptionSetsRequest` method retrieves the metadata. 
1. The `StreamWriter` creates an instance of StreamWriter to write text to a file.

### Clean up

This sample creates no records. No cleanup is required.
