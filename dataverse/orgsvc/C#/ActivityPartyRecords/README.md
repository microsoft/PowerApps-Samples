
# Work with activity party records

This sample code shows how to work with activity party records.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

This sample creates some sample data, to work with activity party records. 

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Creates three contact records which are required for this sample.

### Demonstrate

1. Retrieves the contact records that are created in the [Setup](#Setup). 
2. Creates the activity party records for each contact.
3. Also creates Letter activity and set **From** and **To** columns to the respective Activity Party entities.

### Clean up

Display an option to delete the records created during [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
