---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to perform conditional data operations using the Dataverse Web API."
---
# Web API Conditional Operations sample

This .NET 6.0 sample demonstrates how to perform conditional data operations using the Dataverse Web API.

This sample uses the common helper code in the [WebAPIService](../WebAPIService) class library project.

## Prerequisites

- Microsoft Visual Studio 2022.
- Access to Dataverse with privileges to perform data operations.

## How to run the sample

1. Clone or download the [PowerApps-Samples](../../../../../PowerApps-Samples) repository.
1. Open the [ConditionalOperations.sln](ConditionalOperations.sln) file using Visual Studio 2022.
1. Edit the [appsettings.json](../appsettings.json) file to set the following property values:

   |Property|Instructions  |
   |---------|---------|
   |`Url`|The Url for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. See [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources) to find this. |
   |`UserPrincipalName`|Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access the environment.|
   |`Password`|Replace the placeholder `yourPassword` value with the password you use.|

1. Save the `appsettings.json` file
1. Press F5 to run the sample.

## Demonstrates

This sample has 4 regions:

### Section 0: Create sample records

Operations:

- Create an account record and retrieve it
- Store the initial ETag value from the retrieved record.

### Section 1: Conditional GET

Operations:

- Attempt to retrieve the account record passing the initial ETag value. This operation fails by design by returning a `NotModified` status code value.
- Update the account record.
- Retrieve the account again and store the updated ETag value for use in [Section 2](#section-2-optimistic-concurrency-on-delete-and-update).
- Attempt to retrieve the account record passing the initial ETag value. This operation succeeds because the record was modified since the initial ETag value.

### Section 2: Optimistic concurrency on delete and update

Operations:

- Attempt to delete the account record created passing the initial ETag value. This operation fails by design by returning a `PreconditionFailed` status code value.
- Attempt to update the account record created passing the initial ETag value. This operation fails by design by returning a `PreconditionFailed` status code value.
- Update the account record created passing the *updated* ETag value. 

### Section 3: Delete sample records

Operations: A reference to each record created in this sample was added to a list as it was created. In this sample the records are deleted using a `$batch` operation.

## Clean up

By default this sample will delete all the records created in it.

If you want to view created records after the sample is completed, change the `deleteCreatedRecords` variable to `false` and you will be prompted to decide if you want to delete the records.