---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Sample that shows how to enable and disable auditing on a table and its columns in Microsoft Dataverse. [SOAP]"
---

# Audit table data changes

This sample shows how to enable and disable auditing on a table and its columns, retrieve the data change history of the audited table, and delete the audit records.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `RetrieveRecordChangeHistoryRequest` message is intended to be used in a scenario where it contains data that is needed to retrieve the audit history for a table.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. Creates an sample account table.

### Demonstrate

1. Gets the organization's ID from the system user record.
2. Enabling auditing on organization and also on the sample account table.
3. The `RetrieveRecordChangeHistoryRequest` retrieves the audit history for the account table and displays the result.

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
