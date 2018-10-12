# Sample: Dump entity metadata to a file

This sample shows how to write out all the entity metadata to an `XML` file. It uses the [RetrieveAllEntitiesRequest](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.messages.retrieveallentitiesrequest?view=dynamics-general-ce-9) message.

The following sample creates a new file at `\Entities\bin\Debug\EntityInfo.xml`. You can open this file in **Office Excel** to see a tabular report. You may need this information to discover the entity type code for a custom entity for use in reports.

## How to run this sample

See [How to run samples](../../../How-to-run-samples.md) for information about how to run this sample.

## What this sample does

The `RetrieveAllEntitiesRequest` message is intended to be used in a scenario that contains data that is needed to retrieve metadata information about all the entites.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.


### Demonstrate

1. The `RetrieveAllEntitesRequest` method retrieves the metadata. 
1. The `StreamWriter` creates an instance of StreamWriter to write text to a file.

### Clean up

1. This sample creates no records. No cleanup is required.


