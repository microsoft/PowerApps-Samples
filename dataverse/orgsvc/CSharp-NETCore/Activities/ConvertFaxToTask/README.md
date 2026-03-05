# Convert Fax to Task

This sample shows how to convert a **Fax** to a **Task**.

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

This sample demonstrates how to retrieve a fax activity and create a follow-up task based on the fax information.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Creates a fax activity with the current user as both sender and recipient.

### Demonstrate

1. Retrieves the fax activity created in [Setup](#setup).
2. Creates a task with a subject "Follow Up: [Fax Subject]" and scheduled end date 7 days after the fax creation date.
3. Verifies that the task has been created successfully.

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve the same results.
