
# Sample: Send bulk email and monitor results

This sample shows how to send bulk email using the <xref:Microsoft.Crm.Sdk.Messages.SendBulkMailRequest> and monitor the results by retrieving records from the `AsyncOperation` table.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `SendBulkMailRequest` message is intended to be used in a scenario to send bulk emails.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. Creates 2 contact records for this sample.

### Demonstrate

1. Gets the system user to use as the sender.
2. Set tacking id for the bulk email request.
3. Creates a query expression for the bulk operation to retrieve the contacts in the email list.
4. Set the regarding id and execute the bulk email request.
5. Retrieve the bulk email async operation and monitor it via polling.

### Clean up

Display an option to delete the records created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
