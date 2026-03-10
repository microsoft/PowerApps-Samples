# Determine whether a user has a role

This sample shows how to determine whether a user in Microsoft Dataverse has been associated with a specific role. This is performed by using a query with the [IOrganizationService.RetrieveMultiple](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.retrievemultiple) method.  

This sample requires an additional user that isn't available in your system. Create the required user manually in **Office 365** in order to run the sample without any errors. For this sample create a user profile **as is** shown below. 

**First Name**: Dan<br/>
**Last Name**: Park<br/>
**Security Role**: No security role<br/>
**UserName**: dpark@yourorg.onmicrosoft.com<br/>

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The [IOrganizationService.RetrieveMultiple](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.retrievemultiple) message is intended to be used in a scenario where it retrieves a collection of records.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateRequiredRecords` method creates a user with no security role assigned to him as shown above.

### Demonstrate

1. The `retrieve` method retrieves a user from Dataverse.
2. The `query` message is used to find out a role.

### Clean up

This sample creates no records. No cleanup is required.
