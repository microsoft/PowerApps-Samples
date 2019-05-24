# Quick start for Microsoft Dynamics 365

This sample shows you how to compile and run a program that creates an account record, retrieves the record, updates the record, and then prompts to see if you would like the record deleted.

## How to run this sample

See [How to run samples](../../../How-to-run-samples.md) for information about how to run this sample.

## What this sample does

This sample shows how to perform CRUD operation on an entyty.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.

### Demonstrate

1. The `WhoAMIRequest` method gets the current user's information.
2. The `Account` method creates a smple account record.

### Clean up

1. Display an option to delete the sample data created. The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same resuult.
