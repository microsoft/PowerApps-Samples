# Sample: Retrieve valid status transitions

 This sample shows how to retrieve valid state transitions regardless of whether custom state transitions have been defined for the table.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `GetValidStatusOptions` method is intended to be used in a scenario where it contains data that returns valid status option transitions regardless of whether state transitions are enabled for the table.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `MetadataFilterExpression` method checks for table definitions.

### Demonstrate

1. The `MetadataFilterExpression` method retrieves the status options for the `Incident` table.
1. The `RetrieveMetadataChangeRequest` method retrieves table definitions.
1. The `GetValidStatusOptions` method gets the valid status transitions for each status option.

### Clean up

This sample creates no records. No clean up is required.
