# Dump table privileges information to a file

This sample shows how to write out all the column definitions to an `XML` file. It uses the [RetrieveAllEntitiesRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrieveallentitiesrequest) message.

The following sample creates a new file at `\DumpEntityPriviliges\bin\Debug\EntityPrivileges.xml`. You can open this file in **Office Excel** to see a tabular report. 

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `RetrieveAllEntitiesRequest` message is intended to be used in a scenario that contains data that is needed to retrieve table definitions information.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `RetrieveAllEntitiesRequest` method retrieves the column definitions. 
1. The `StreamWriter` creates an instance of StreamWriter to write text to a file.

### Clean up

This sample creates no records. No cleanup is required.