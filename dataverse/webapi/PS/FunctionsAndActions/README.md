---
languages:
- powershell
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to use Dataverse Web API functions and actions using PowerShell."
---
# Web API PowerShell Functions and Actions sample

This PowerShell sample demonstrates how to use Dataverse Web API functions and actions, including:

- Unbound functions (WhoAmI, FormatAddress, InitializeFrom, RetrieveCurrentOrganization, RetrieveTotalRecordCount)
- Bound functions (IsSystemAdmin custom API, RetrievePrincipalAccess)
- Unbound actions (GrantAccess)
- Bound actions (AddPrivilegesRole)

This sample uses the [Dataverse Web API PowerShell Helper functions](../README.md) to manage authentication and provide re-usable functions to perform common operations. These scripts are referenced using [dot sourcing](https://learn.microsoft.com/powershell/module/microsoft.powershell.core/about/about_scripts#script-scope-and-dot-sourcing) with the following lines:

```powershell
. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1
. $PSScriptRoot\..\CommonFunctions.ps1
```

> [!NOTE]
> This sample should work with Windows, Linux, and macOS, but has only been tested on Windows.

## Prerequisites

Before running this sample you should read these articles that explain concepts and patterns used by these samples:

- [Quick Start Web API with PowerShell and Visual Studio Code](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/quick-start-ps)
- [Use PowerShell and Visual Studio Code with the Dataverse Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/use-ps-and-vscode-web-api)
- [Use Web API functions](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/use-web-api-functions)
- [Use Web API actions](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/use-web-api-actions)

This sample requires:

- Visual Studio Code. See [Download Visual Studio Code](https://code.visualstudio.com/download)
- PowerShell extension for Visual Studio Code. See [PowerShell for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell)
- PowerShell 7.4 or higher. See [Install PowerShell on Windows, Linux, and macOS](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
- Az PowerShell module version 11.1.0 or higher. See [How to install Azure PowerShell](https://learn.microsoft.com/powershell/azure/install-azure-powershell)

   [To update an existing installation to the latest version](https://learn.microsoft.com/powershell/module/powershellget/update-module), use `Update-Module -Name Az -Force`

- Access to Dataverse with privileges to perform data operations and manage security roles.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the [FunctionsAndActions.ps1](FunctionsAndActions.ps1) file using Visual Studio Code
1. Edit this line to use the URL of the environment you want to connect to:

   `Connect 'https://yourorg.crm.dynamics.com/' # change this`

1. (Optional) Set the `$deleteCreatedRecords` variable to `$false` if you do not want to delete the records this sample creates.
1. Press F5 to run the sample.
1. The first time you run the sample a browser window opens. In the browser window, enter or select the credentials you want to use to authenticate.

To connect as a different user, run the [Disconnect-AzAccount command](https://learn.microsoft.com/powershell/module/az.accounts/disconnect-azaccount).

## Demonstrates

This sample has 9 sections:

### Setup: Create sample records

Operations: Create sample data for the function and action demonstrations.

- Create an account record named 'Contoso Consulting' with complete address information
- Create an account record named 'Account to Share' for the GrantAccess demonstration

All records are added to the `$recordsToDelete` array for cleanup.

### Section 1: Unbound Function WhoAmI

Operations: Use the WhoAmI function to retrieve information about the current user.

- Call `Get-WhoAmI` function
- Display `BusinessUnitId`, `UserId`, and `OrganizationId` values

### Section 2: Unbound Function FormatAddress

Operations: Use the FormatAddress function to format addresses according to country/regional requirements.

- Format a US address using `Format-Address` function
- Format a Japan address using `Format-Address` function
- Observe how different countries format addresses differently

### Section 3: Unbound Function InitializeFrom

Operations: Use the InitializeFrom function to create a new record based on an existing record with mapped column values.

- Call `Initialize-From` function to get default values from the 'Contoso Consulting' account
- Modify the initialized data with new values for a Chicago branch
- Create the new account record using `New-Record` function

### Section 4: Unbound Function RetrieveCurrentOrganization

Operations: Use the RetrieveCurrentOrganization function to get detailed information about the current organization.

- Call `Get-CurrentOrganization` function
- Display organization details including `OrganizationId`, `FriendlyName`, `OrganizationVersion`, `EnvironmentId`, `UrlName`, and `UniqueName`

### Section 5: Unbound Function RetrieveTotalRecordCount

Operations: Use the RetrieveTotalRecordCount function to get the total number of records for specified entities.

- Call `Get-TotalRecordCount` function for 'account' and 'contact' entities
- Display record counts from a snapshot taken within the last 24 hours

### Section 6: Bound Function IsSystemAdmin

Operations: Use the `sample_IsSystemAdmin` custom API to check if users have the System Administrator role.

- Check if the `IsSystemAdminFunction` solution is installed
- If installed, retrieve top 10 enabled interactive users
- Call `Test-SystemAdministrator` function for each user
- Display whether each user has the System Administrator role

> [!NOTE]
> This section requires the `IsSystemAdminFunction` solution to be installed. If not installed, this section will install it. See [IsSystemAdmin custom API sample plug-in](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/samples/issystemadmin-customapi-sample-plugin) for more information.

### Section 7: Unbound Action GrantAccess

Operations: Use the GrantAccess action to grant access rights to another user for a specific record.

- Find an enabled user other than the current user
- Call `Get-PrincipalAccess` function to check current access rights
- If the user doesn't have DeleteAccess, call `Grant-Access` function to grant it
- Verify the access was granted by calling `Get-PrincipalAccess` again

### Section 8: Bound Action AddPrivilegesRole

Operations: Use the AddPrivilegesRole action to add privileges to a security role.

- Create a new security role named 'Test Role'
- Display initial privileges in the role
- Retrieve the `prvCreateAccount` and `prvReadAccount` privileges
- Call `Add-RolePrivilege` function to add the privileges to the role
- Display updated privileges in the role

### Section 9: Delete sample records

Operations: A reference to each record created in this sample was added to a list as it was created.

When the `$deleteCreatedRecords` variable is set to `$true`, this section loops through that list in reverse order and deletes each record using the `Remove-Record` function.

## New functions in CommonFunctions.ps1

This sample adds the following functions to `CommonFunctions.ps1`:

- `Format-Address` - Formats an address according to country/region-specific requirements
- `Initialize-From` - Initializes a new record from an existing record based on mapping configuration
- `Get-CurrentOrganization` - Retrieves detailed information about the current organization
- `Get-TotalRecordCount` - Retrieves the total number of records for specified entities
- `Test-SystemAdministrator` - Determines whether a user has the System Administrator security role
- `Get-PrincipalAccess` - Retrieves the access rights a principal has to a specific record
- `Grant-Access` - Grants access rights to a principal for a specific record
- `Add-RolePrivilege` - Adds privileges to a security role

All these functions use `Invoke-ResilientRestMethod` internally to handle API throttling and retry logic.

## Clean up

By default, this sample deletes all the records it created. If you want to view created records after the sample is complete, change the `$deleteCreatedRecords` variable to `$false`.
