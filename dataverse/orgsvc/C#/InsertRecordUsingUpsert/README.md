# Sample: Insert or update a record using Upsert

This sample code shows how to insert or update records by using the [UpsertRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.upsertrequest) message. 

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.
## What this sample does

The `UpsertRequest` message is intended to be used in a scenario where it contains data that is needed to update or insert a record.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. Import a managed solution (UpsertSample_1_0_0_0_managed.zip) that creates a `sample_product` table that has an alternate key named `sample_productcode`. Verify that the indexes to support the alternate key are active.

### Demonstrate

1. The `ProcessUpsert` method processes data in the `newsampleproduct.xml` to represent new products and creates 13 new records.
1. The second time when the `ProcessUpsert` method is called, it processes data in `updatedsampleproduct.xml` to represent updates to products previously created. 
1. The `UpsertRequest` method creates 6 updated records. 

### Clean up

Display an option to delete the managed solution created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
