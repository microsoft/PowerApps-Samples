---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample shows how to perform a bulk deletion of records that were previously exported from Microsoft Dataverse by using the Export to Excel option. [SOAP]"
---

# Bulk delete exported records

This sample shows how to perform a bulk deletion of records that were previously exported from Dynamics 365 by using the **Export to Excel** option.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `BulkDeleteRequest` message is intended to be used in a scenario where it contains data that is needed to create the bulk delete request.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. Query for a system user to send an email after bulk delete request operation completes.
3. The `BulkDeleteRequest` creates the bulk delete process and set the request properties.
4. The `CheckSuccess` method queries for the `BulkDeleteOperation` until it has been completed or until the designated time runs out. It then checks to see if the operation is complete.

### Demonstrate

The `PerformBulkDeleteBackup` method performs the main ulk delete operation on inactive opportunities and activities to remove them from the system.

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
