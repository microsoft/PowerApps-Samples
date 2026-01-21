# Remove a role for a user

This sample shows how to disassociate a role from a user by using the [IOrganizationService.Disassociate](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.disassociate) method. 

This sample requires an additional user that isn't available in your system. Create the required user manually in **Office 365** in order to run the sample without any errors. For this sample create a user profile **as is** shown below. 

**First Name**: Dan<br/>
**Last Name**: Park<br/>
**Security Role**: No security role<br/>
**UserName**: dpark@yourorg.onmicrosoft.com<br/>

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The [IOrganizationService.Disassociate](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.disassociate) message is intended to be used in a scenario where it deletes a link between records.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateRequiredRecords` method creates the records required by the sample.

### Demonstrate

1. The `query` method retrieves a role from Microsoft Dataverse.
2. The `Disassociate` message removes the role to a team.

### Clean up

This sample creates no records. No cleanup is required.
