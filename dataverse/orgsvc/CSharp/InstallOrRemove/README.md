---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample shows how to install or uninstall the sample data for an organization by using the InstallSampleDataRequest message in Microsoft Dataverse. [SOAP]"
---

# Install or remove sample data

This sample shows how to install or uninstall the sample data for an organization by using the [InstallSampleDataRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.installsampledatarequest) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `InstallSampleDataRequest` message is intended to be used in a scenario where it contains data that is needed to install the sample data.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.

### Demonstrate

1. Prompts users to install or remove sample data.
2. If the user opts to install sample data, the `InstallSampleDataRequest` message installs the sample data.
3. If the user opts to uninstall the sample data, the `UninstallSampleDataRequest` message removes the sample data.

### Clean up

Display an option to delete the records in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
