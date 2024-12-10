# Convert Fax to Task

This sample shows how to convert a **Fax** to a **Task**.

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

What this sample does

The `CreateRequiredRecords` creates the data required for the sample. The `retrievedFax` retrieves the fax. 
The `DeleteRequiredRecords` gives an option to delete all the data that sample has created.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current  version of the org. 
1. Creates required data that this sample requires.

### Demonstrate

1. Retrieves the fax id's that are created in [Setup](#setup).
2. Creates a task and verifies that the task has been created.

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.
