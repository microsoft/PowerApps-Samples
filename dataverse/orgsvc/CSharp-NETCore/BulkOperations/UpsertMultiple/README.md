# UpsertMultiple README

This project uses [UpsertMultipleRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.upsertmultiplerequest) 
class to perform bulk upsert operations.

## Create table for sample

This sample will create a new table that the code will use. It depends on the common structure for other projects in this solution that is described in [BulkOperations/README.md](../README.md).

This sample demonstrates upserting records using both primary and alternate keys. 

For standard tables, the `CreateAlternateKey` property in Settings.cs controls whether an alternate key will be created for the table. The default value is `true`. The name of the column defined as an alternate key is `sample_keyattribute`.

Elastic tables have a single alternate key defined and you can't create a new one. When configured for elastic tables, this sample will use the primary key and `partitionid` columns to uniquely identify records.

## Preparing data for upsert

To demonstrate upsert, there needs to be some records in the system to be updated. Depending on the `NumberOfRecords` specified in Settings.cs, half that number are created in the first step.

Then, entity instances for the total number of records are prepared.

- &frac14; have no primary or alternate key set. Only the `sample_name` property has data. These will be *created* during Upsert.
- &frac14; have a unique value set for the `sample_keyattribute` alternate key. These will be *created* during Upsert.
- &frac14; have primary key values from the records already created. These will be *updated* during Upsert.
- &frac14; have alternate key values set from the records already created. These will be *updated* during Upsert.

## Checking Alternate key

Creating an alternate key takes some time because the data in the column needs to be indexed. This sample verifies that the alternate key is ready before continuing. This needs to be done if you are using an alternate key immediately after you create it.

More information: [Monitor index creation for alternate keys](https://learn.microsoft.com/power-apps/developer/data-platform/define-alternate-keys-entity#monitor-index-creation-for-alternate-keys)

## Executing UpsertMultiple and validating responses

After the prepared collection of entity instances is sent using `UpsertMultiple`, the sample inspects the 
[UpsertMultipleResponse.Results](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.upsertmultipleresponse.results) property, which is an array of [UpsertResponse](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.upsertresponse). The [UpsertResponse.RecordCreated](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.upsertresponse.recordcreated) boolean property tells you whether the operation resulted in an Create or Update operation.

## Deleting records

All the records created will be deleted by default. You might set a breakpoint here or comment out this code if you want to examine the records.

## Deleting the sample table

When the `DeleteTable` property in Settings.cs is true, the table is deleted. You can change this setting if you want to run the sample multiple times.

## Sample output

The full sample output should look like this:

```
Creating sample_Example Standard table...
        sample_Example table created.
Adding 'sample_Description' column to sample_Example table...
        'sample_Description' column created.

Preparing 50 entities to create..
        Created 50 records to upsert..

        Preparing 25 entity instances to create without alternate key value.
        Preparing 25 entity instances to create with an alternate key value

        Preparing 25 entity instances to update using existing Primary Key value..
        Preparing 25 entity instances to update  with existing alternate key value..

Check if AlternateKey is available in the metadata for the table...
        Alternate key is available

Sending UpsertMultipleRequest...
        Upserted 100 records in 2 seconds.
Records Created: 50, Records Updated: 50

Starting asynchronous bulk delete of 100 created records...
        Asynchronous job to delete 100 records completed in 16 seconds.
        Bulk Delete status: Succeeded

Deleting sample_Example table...
        sample_Example table deleted.
```