---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to use OData Functions and Actions using the Dataverse Web API."
---

# Web API Functions and Actions sample

This .NET 6.0 sample demonstrates how to use OData Functions and Actions using the Dataverse Web API.

This sample uses the common helper code in the [WebAPIService](../WebAPIService) class library project.

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with privileges to perform data operations

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the [FunctionsAndActions.sln](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/FunctionsAndActions/FunctionsAndActions.sln) file using Visual Studio 2022.
1. Edit the [appsettings.json](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/appsettings.json) file to set the following property values.

   | Property | Instructions |
   |----------|--------------|
   | `Url` | The URL for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. Learn how to find your URL in [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources). |
   | `UserPrincipalName` | Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access the environment. |
   | `Password` | Replace the placeholder `yourPassword` value with the password you use. |

1. Save the `appsettings.json` file
1. Press `F5` to run the sample.

## Demonstrates

This sample has nine sections:

- [Unbound Functions: WhoAmI](#section-1-unbound-functions-whoami)
- [Unbound Functions: FormatAddress](#section-2-unbound-functions-formataddress)
- [Unbound Functions: InitializeFrom](#section-3-unbound-functions-initializefrom)
- [InitializeFrom](#section-4-initializefrom)
- [Unbound Functions: RetrieveTotalRecordCount](#section-5-unbound-functions-retrievetotalrecordcount)
- [Bound Functions: IsSystemAdmin](#section-6-bound-functions-issystemadmin)
- [Unbound Actions: GrantAccess](#section-7-unbound-actions-grantaccess)
- [Bound Actions: AddPrivilegesRole](#section-8-bound-actions-addprivilegesrole)
- [Delete sample records](#section-9-delete-sample-records)

### Section 1: Unbound Functions: WhoAmI

Operations: Use the [WhoAmI Function](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/whoami) with the WebAPIService [WhoAmIRequest](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/WhoAmIRequest.cs) and [WhoAmIResponse](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/WhoAmIResponse.cs) classes.

### Section 2: Unbound Functions: FormatAddress

Operations: Use the [FormatAddress Function](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/formataddress) with the WebAPIService [FormatAddressRequest](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/FormatAddressRequest.cs) and [FormatAddressResponse](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/FormatAddressResponse.cs) classes.

### Section 3: Unbound Functions: InitializeFrom

Operations: Use the [InitializeFrom Function](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/initializefrom) with the WebAPIService [InitializeFromRequest](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/InitializeFromRequest.cs) and [InitializeFromResponse](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/InitializeFromResponse.cs) classes.

### Section 4: Unbound Functions: RetrieveCurrentOrganization

Operations: Use the [RetrieveCurrentOrganization Function](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/retrievecurrentorganization) with the WebAPIService [RetrieveCurrentOrganizationRequest](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/RetrieveCurrentOrganizationRequest.cs) and [RetrieveCurrentOrganizationResponse](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/RetrieveCurrentOrganizationResponse.cs) classes.

### Section 5: Unbound Functions: RetrieveTotalRecordCount

Operations: Use the [RetrieveTotalRecordCount Function](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/retrievetotalrecordcount) with the WebAPIService [RetrieveTotalRecordCountRequest](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/RetrieveTotalRecordCountRequest.cs) and [RetrieveTotalRecordCountResponse](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/RetrieveTotalRecordCountResponse.cs) classes.

### Section 6: Bound Functions: IsSystemAdmin

Operations: Use a custom `sample_IsSystemAdmin` operation created using Custom API as a function bound to the `systemuser` table.

This code installs a managed solution containing the Custom API. The code uses the [IsSystemAdminRequest](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/FunctionsAndActions/Messages/IsSystemAdminRequest.cs) and [IsSystemAdminResponse](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/FunctionsAndActions/Messages/IsSystemAdminResponse.cs) classes defined within this sample application.

This sample retrieves a set of users and tests each one to determine whether the System Administrator security role is associated with their `systemuser` record or team.

[Sample: IsSystemAdmin Custom API](../../../orgsvc/CSharp/IsSystemAdminCustomAPI/) describes the Custom API used by this sample.

### Section 7: Unbound Actions: GrantAccess

Operations: Use the [GrantAccess Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/grantaccess) with the WebAPIService [GrantAccessRequest](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/GrantAccessRequest.cs) class.

### Section 8: Bound Actions: AddPrivilegesRole

Operations: Use the [AddPrivilegesRole Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/addprivilegesrole) with the WebAPIService [AddPrivilegesRoleRequest](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/AddPrivilegesRoleRequest.cs) class.

### Section 9: Delete sample records

Operations: A reference to each record created in this sample is added to a list as it's created. In this sample, the records are deleted using a `$batch` operation.

## Clean up

By default this sample deletes all the records created in it.

If you want to view created records after the sample is completed, change the `deleteCreatedRecords` variable to `false` and you're be prompted to delete the records if desired.
