# UpsertMultiple README

This project uses the [UpsertMultiple](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/upsertmultiple) action to demonstrate bulk upsert operations.

## Create table for sample

This sample will create a new table that the code will use. It depends on the common structure for other projects in this solution that is described in [BulkOperations/README.md](../README.md).

When configured for standard tables, this sample demonstrates upserting records using both primary and alternate keys. The `CreateAlternateKey` property in Settings.cs controls whether an alternate key will be created for the table. The default value is `true`. The name of the column defined as an alternate key is `sample_keyattribute`.

Elastic tables have a single alternate key defined and you can't create a new one. When configured for elastic tables, this sample will use the `partitionid` column to uniquely identify records.

## Preparing data for upsert

To demonstrate upsert, there needs to be some records in the system to be updated. Depending on the `NumberOfRecords` specified in Settings.cs, half that number are created in the first step.

Then, entity instances for the total number of records are prepared.

- &frac14; have no primary or alternate key set. Only the `sample_name` property has data. These will be *created* during Upsert.
- &frac14; have a unique value set for the `sample_keyattribute` alternate key for standard tables or `partitionid` for elastic tables. These will be *created* during Upsert.
- &frac14; have primary key values from the records already created. These will be *updated* during Upsert.
- &frac14; have alternate key values set from the records already created. These will be *updated* during Upsert.

## Checking Alternate key

Creating an alternate key takes some time because the data in the column needs to be indexed. This sample verifies that the alternate key is ready before continuing. This needs to be done if you are using an alternate key immediately after you create it.

More information: [Monitor index creation for alternate keys](https://learn.microsoft.com/power-apps/developer/data-platform/define-alternate-keys-entity#monitor-index-creation-for-alternate-keys)

## Executing UpsertMultiple

After the prepared collection of entity instances is sent using the [UpsertMultiple](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/upsertmultiple) action. The [UpsertMultipleResponse](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/upsertmultipleresponse) complex type is not returned. 

## Deleting records

All the records created will be deleted by default. You might set a breakpoint here or comment out this code if you want to examine the records.

## Deleting the sample table

When the `DeleteTable` property in Settings.cs is true, the table is deleted. You can change this setting if you want to run the sample multiple times.

## Sample output

The full sample output should look like this:

```
Creating sample_Example Standard table...

Preparing 50 records to create..
Sending POST request to /sample_examples/Microsoft.Dynamics.CRM.UpsertMultiple

Preparing 50 records to Upsert(update)..

Preparing 25 records to Upsert(create) using Primary Key..

Preparing 25 records to Upsert(create) using Alternate Key..
Sending POST request to /sample_examples/Microsoft.Dynamics.CRM.UpsertMultiple
        Upserted 100 records in 1 seconds.

Starting asynchronous bulk delete of 100 upserted records...
        Asynchronous job to delete 100 records completed in 14 seconds.
        Bulk Delete status: Succeeded


Deleting sample_Example table...
        sample_Example table deleted.
```