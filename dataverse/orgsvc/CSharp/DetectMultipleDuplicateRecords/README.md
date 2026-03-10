---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample shows how to detect and log multiple duplicate records for a specified table in Microsoft Dataverse. [SOAP]"
---

# Detect multiple duplicate records

This sample shows how to detect and log multiple duplicate records for a specified table.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `BulkDetectDuplicatesRequest` message is intended to be used in a scenario that contains data that is needed to submit an asynchronous system job that detects and logs multiple duplicate records.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `CreateRequiredRecords` class creates some duplicate table records for the sample.
1. The `DuplicateRule` method creates a duplicate detection rule.
1. The  `DuplicateRuleCondition` method creates a duplicate detection rule condition for detecting duplicate records.
1. The `PublishDuplicateRuleRequest` method publishes the duplicate detection rule.
1. The `PublishDuplicateRuleRequest` returns before the publish is completed, so we keep retrieving the async job state until it is `Completed`

### Demonstrate

The `BulkDetectDuplicatesRequest` method creates the BulkDetectDuplicatesRequest object

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.

