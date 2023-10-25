# Sample: Use optimistic concurrency with update and delete operations

This sample shows how to use optimistic concurrency for update and delete operations.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `UpdateRequest` class is intended to be used in a scenario where it contains data that is needed to update an existing record.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `Account` method creates an account record.

### Demonstrate

1. Retrieves the account record that is created in the [Setup](#setup).
1. Updates the account record by increasing the `creditlimit` attribute.
1. The `UpdateRequest` method sets the request's concurrency behavior to check for a row version match.

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.
