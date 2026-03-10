# Associate security role to a user

This sample shows how to assign a security role to a user by using the [IOrganizationService.Associate](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice) message. 

This sample requires an additional user that isn't available in your system. Create the required user manually in **Office 365** in order to run the sample without any errors. For this sample create a user profile **as is** shown below. 

**First Name**: Dan<br/>
**Last Name**: Park<br/>
**Security Role**: User without any roles assigned<br/>
**UserName**: dpark@yourorg.onmicrosoft.com<br/>

## How to run this sample

See [How to run this sample](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The [IOrganizationService.Associate](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice) message is intended to be used in a scenario where it provides programmatic access to the metadata and data for an organization.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateRequiredRecords` method creates the records required by the sample.

### Demonstrate

1. The `QueryExpression` method retrieves a role from Microsoft Dataverse.
2. The `Associate` message assigns the role to a user.

### Clean up

Display an option to delete the sample data in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
