# Export ribbon definitions

This sample shows how to export Ribbon definitions. It uses the [RetrieveApplicationRibbonRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.retrieveapplicationribbonrequest) and [RetrieveEntityRibbonRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.retrieveentityribbonrequest) messages. You can find the downloaded ribbon definitions in `ExportRibbonDefinitions\bin\Debug`.

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `RetrieveApplicationRibbonRequest` message is intended to be used in a scenario where it contains data that is needed to retrieve the data that defines the content and behavior of the application ribbon. The `RetrieveEntityRibbonRequest` message is intended to be used in a scenario where it contains data that is needed to retrieve ribbon definitions for a table.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `RetrieveApplicationRibbonRequest` method retrieves the application ribbon.
2. The `RetrieveEntityRibbonRequest` method retrieves the system table ribbons

### Clean up

No clean up is required for this sample
