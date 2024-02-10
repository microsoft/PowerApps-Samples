# UpsertMultiple README

This project uses the [UpsertMultiple](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/upsertmultiple) action to demonstrate bulk upsert operations.

## Create table for sample

This sample will create a new table that the code will use. It depends on the common structure for other projects in this solution that is described in [BulkOperations/README.md](../README.md).

When configured for standard tables, this sample demonstrates upserting records using both primary and alternate keys. The `CreateAlternateKey` property in Settings.cs controls whether an alternate key will be created for the table. The default value is `true`. The name of the column defined as an alternate key is `sample_keyattribute`.

Elastic tables have a single alternate key defined and you can't create a new one. When configured for elastic tables, this sample will use the primary key and `partitionid` columns to uniquely identify records.

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

Because of the different configuration options you have when running this sample and how the different behaviors are demonstrated on &frac14; of the total number of records sent, you can send a group of 4 records to see the differences. The following sections show the JSON sent in the `Targets` parameter for just four records and different settings.

### `UseElastic` `false` with `CreateAlternateKey` `false` (default)

Standard table without alternate key.

```json
[
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "sample_name": "sample record 0000001 UpdatedByPK",
    "sample_exampleid": "7a282686-47c8-ee11-9079-000d3a993550"
  },
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "sample_name": "sample record 0000002 UpdatedByPK",
    "sample_exampleid": "7b282686-47c8-ee11-9079-000d3a993550"
  },
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "sample_name": "sample record 0000003",
    "sample_exampleid": "403e5602-5ba2-4df6-b878-5b08c8e23d22",
    
  },
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "sample_name": "sample record 0000004",
    "sample_exampleid": "3a7270aa-ed34-48d4-a02f-3ccb62f0b961",
  }
]
```


### `UseElastic` `false` with `CreateAlternateKey` `true`

Standard table with alternate key.

```json
[
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "sample_name": "sample record 0000001 UpdatedByPK",
    "sample_exampleid": "28293eb2-49c8-ee11-9079-000d3a993550"
  },
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "@odata.id": "sample_examples(sample_keyattribute='0000002')"
    "sample_name": "sample record 0000002 UpdatedByAK"
  },
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "sample_name": "sample record 0000003",
    "sample_exampleid": "94d8687d-b3cf-4d1e-9330-855848aa5980"
  },
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "@odata.id": "sample_examples(sample_keyattribute='0000004')",
    "sample_name": "sample record 0000004"
  }
]
```


### `UseElastic` `true` with `CreateAlternateKey` `false`

Elastic table without using only primary key.

```json
[
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "sample_name": "sample record 0000001 UpdatedByPK",
    "partitionid": "PARTITION_VALUE",
    "sample_exampleid": "505673c8-4ac8-ee11-9079-000d3a993550"
  },
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "sample_name": "sample record 0000002 UpdatedByPK",
    "partitionid": "PARTITION_VALUE",
    "sample_exampleid": "515673c8-4ac8-ee11-9079-000d3a993550"
  },
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "sample_name": "sample record 0000003",
    "partitionid": "PARTITION_VALUE",
    "sample_exampleid": "d9e34747-28c5-4fae-b643-be86af2722aa"
  },
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "sample_name": "sample record 0000004",
    "partitionid": "PARTITION_VALUE",
    "sample_exampleid": "3fb7e861-56bb-45e3-9197-ad268c962e51"
  }
]
```

### `UseElastic` `true` with `CreateAlternateKey` `true`

Elastic table using primary key and `partititionid`.

```json
[
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "sample_name": "sample record 0000001 UpdatedByPK",
    "partitionid": "PARTITION_VALUE",
    "sample_exampleid": "f755017c-4bc8-ee11-9079-000d3a993550"
  },
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "@odata.id": "sample_examples(sample_exampleid=f855017c-4bc8-ee11-9079-000d3a993550,partitionid='PARTITION_VALUE')",
    "sample_name": "sample record 0000002 UpdatedByAK"
  },
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "sample_name": "sample record 0000003",
    "sample_exampleid": "3d9486ce-e5f8-4222-8010-2d9957548c4a",
    "partitionid": "PARTITION_VALUE"
  },
  {
    "@odata.type": "Microsoft.Dynamics.CRM.sample_example",
    "@odata.id": "sample_examples(sample_exampleid=5012e72e-e9f0-426e-9fb2-57f7c97d6aff,partitionid='PARTITION_VALUE')",
    "sample_name": "sample record 0000004",
    "partitionid": "PARTITION_VALUE"    
  }
]
```


## Deleting records

All the records created will be deleted by default. You might set a breakpoint here or comment out this code if you want to examine the records.

## Deleting the sample table

When the `DeleteTable` property in Settings.cs is true, the table is deleted. You can change this setting if you want to run the sample multiple times.

## Sample output

The full sample output using the default settings should look like this:

```
Creating sample_Example Standard table...

Preparing 50 records to create..
Sending POST request to /sample_examples/Microsoft.Dynamics.CRM.CreateMultiple

Preparing 50 records to Upsert(update)..

Preparing 50 records to Upsert(create) using Primary Key..
Sending POST request to /sample_examples/Microsoft.Dynamics.CRM.UpsertMultiple
        Upserted 100 records in 1 seconds.

Starting asynchronous bulk delete of 100 upserted records...
        Asynchronous job to delete 100 records completed in 13 seconds.
        Bulk Delete status: Succeeded


Deleting sample_Example table...
        sample_Example table deleted.
```