# Rollup records related to a specific record

This sample shows how to roll up opportunities by the parent account.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `RollupRequest` message is intended to be used in a scenario where it contains data that is needed to create a roll up request.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. Creates sample account and opportunity records.

### Demonstrate

1. The `QueryExpression` queries the opportunities by parent account.
2. The `RollupRequest` creates the roll up request.

### Clean up

Display an option to delete the records in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
