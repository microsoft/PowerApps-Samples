# Get report history limits

This sample shows how to get report history limits using the [GetReportHistoryLimitRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.getreporthistorylimitrequest) message.

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `GetReportHistoryLimitRequest` message is intended to be used in a scenario where it contains data that is needed to retrieve the history limit for a report.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `QueryByAttribute` method  queries for an existing report.
2. The `GetReportHistoryLimitRequest` method gets the history limit data.

### Clean up

No clean up is required for this sample.