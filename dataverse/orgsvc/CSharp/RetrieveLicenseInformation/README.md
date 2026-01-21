# Retrieve license information

This sample shows how to use the [IDeploymentService.RetrieveDeploymentLicenseTypeRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.retrievedeploymentlicensetyperequest) message and the [IOrganizationService.RetrieveLicenseInfoRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.retrievelicenseinforequest) message to retrieve information about licenses.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The [IDeploymentService.RetrieveDeploymentLicenseTypeRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.retrievedeploymentlicensetyperequest) message is intended to be used in a scenario where it contains data  that is needed to retrieve the type of license for a deployment of Microsoft Dataverse.

The [IOrganizationService.RetrieveLicenseInfoRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.retrievelicenseinforequest) message is intended to be used in a scenario where it contains data that is needed to retrieve the number of used and available licenses for a deployment of Dataverse.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

Checks for the current version of the org.

### Demonstrate

1. The `deploymentTypeRequest` method creates a request to retrieve the deployment license types.
2. The `licenseInfoRequest` message creates request to retrieve the licensed info request.

### Clean up

This sample creates no records. No cleanup is required.