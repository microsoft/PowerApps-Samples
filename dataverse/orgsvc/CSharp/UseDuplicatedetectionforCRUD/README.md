# Sample: Use duplicate detection when creating and updating records

This sample shows how to invoke duplicate detection for creating and updating table records.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `DuplicateRule` method is intended to be used in a scenario where it contains data that is needed to create duplicate detection rule.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `Account` method creates an account record. 
1. The `DuplicateRule` method creates a duplicate detection rule.
1. The `DuplicateRuleCondition` method creates a duplicate detection rule conditions.
1. The `PublishDuplicateRuleRequest` method publishes the duplicate detection rule created earlier. You need to wait for publishing operation to complete, so we keep polling the state of the rule until it becomes `Published`.

### Demonstrate

1. The `Account` method creates an account record. 
1. The `CreateRequest` method creates operation by suppressing duplicate detection.
1. The `UpdateRequest` method updates the retrieved account record with new account number.

### Clean up

Displays an option to delete all the data created in the sample. The deletion is optional in case you want to examine the data created by the sample. You can manually delete the data to achieve same results.
