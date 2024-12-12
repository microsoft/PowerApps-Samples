# Sample: Import data using complex data map

This sample shows how to create new records by using data import. The sample uses a complex data map.

>[!NOTE]
> The source data for this sample is contained in the following file `ImportComplexDataMap\import accounts.csv`

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `ImportMap` method creates an import map.
1. The `ColumnMapping` method creates a column mapping for a `text` type column.
1. The `EntityReference` method relates the column mapping with the data map.
1. The `LookUpMapping` method creates a lookup mapping to the parent account.
1. The `ImportFile` method creates a import file.
1. The `GetHeaderColumnsImportFileRequest` method retrieves the header columns used in the import file.
1. The `ParseImportRequest` method parses the import file. 
1. The `RetrievedParsedDataImportFileRequest` method retrieves the data from the parse table.
1. The `TransformImportRequest` method transforms the import.


### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.

