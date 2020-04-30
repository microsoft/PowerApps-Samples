# Export ribbon definitions

This sample shows how to export Ribbon definitions. It uses the [RetrieveApplicationRibbonRequest](https://docs.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.retrieveapplicationribbonrequest?view=dynamics-general-ce-9) and [RetrieveEntityRibbonRequest](https://docs.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.retrieveentityribbonrequest?view=dynamics-general-ce-9) messages. You can find the downloaded reibbon definitions in `ExportRibbonDefinitions\bin\Debug`.

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

The `RetrieveApplicationRibbonRequest` message is intended to be used in a scenario where it contains data that is needed to retrieve the data that defines the content and behavior of the application ribbon. The `RetrieveEntityRibbonRequest` message is intended to be used in a scenario where it contains data that is needed to retrieve ribbon definitions for an entity.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `RetrieveApplicationRibbonRequest` method retrieves the application ribbon.
2. The `RetrieveEntityRibbonRequest` method retrieves the system entity ribbons

### Clean up

No clean up is required for this sample
