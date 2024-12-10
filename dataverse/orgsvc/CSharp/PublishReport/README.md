# Publish reports

This sample shows how to publish a report by creating a **Report** record and the related records that make it visible..

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## How this sample works

In order to simulate the scenario described above, the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `Report` method instantiates the report object.
2. The `ReportCategory` method sets the report category te report should belong to.
3. The `ReportEntity` method defines which table this report uses.
4. The `ReportVisibility` method sets the report visibility.

### Clean up

No clean up is required for this sample.