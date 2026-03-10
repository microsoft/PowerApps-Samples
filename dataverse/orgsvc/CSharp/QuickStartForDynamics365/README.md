# Quick start for Microsoft Dataverse (Organization Service)

This sample shows you how to compile and run a program that creates an account record, retrieves the record, updates the record, and then prompts to see if you would like the record deleted.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

This sample shows how to perform CRUD operation on a table.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `WhoAMIRequest` method gets the current user's information.
2. The `Account` method creates a sample account record.

### Clean up

Display an option to delete the sample data created. The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
