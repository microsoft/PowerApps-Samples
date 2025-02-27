---
languages:
- powershell
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to perform common data operations using the Dataverse Web API using PowerShell with Visual Studio Code."
---
# Dataverse Web API PowerShell Basic Operations sample

This PowerShell version 7.4.0 sample demonstrates how to perform common data operations using the Dataverse Web API and Visual Studio Code.

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

This sample requires:

- Visual Studio Code. See [Download Visual Studio Code](https://code.visualstudio.com/download)
- PowerShell extension for Visual Studio Code. See [PowerShell for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell)
- PowerShell 7.4 or higher. See [Install PowerShell on Windows, Linux, and macOS](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
- Az PowerShell module version 11.1.0 or higher. See [How to install Azure PowerShell](https://learn.microsoft.com/powershell/azure/install-azure-powershell)

   [To update an existing installation to the latest version](https://learn.microsoft.com/powershell/module/powershellget/update-module), use `Update-Module -Name Az -Force`

- Access to Dataverse with privileges to perform data operations.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the [BasicOperations.ps1](BasicOperations.ps1) file using Visual Studio Code
1. Edit this line to use the URL of the environment you want to connect to:

   `Connect 'https://yourorg.crm.dynamics.com/' # change this`

1. (Optional) Set the `$deleteCreatedRecords` variable to `$false` if you do not want to delete the records this sample creates.
1. Press F5 to run the sample.
1. The first time you run the sample a browser window opens. In the browser window, enter or select the credentials you want to use to authenticate.

To connect as a different user, run the [Disconnect-AzAccount command](https://learn.microsoft.com/powershell/module/az.accounts/disconnect-azaccount).

## Demonstrates

This sample has 5 regions that demonstrate common data operations described in detail within the [HTTP Web API Basic Operations Sample](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-basic-operations-sample)

To begin, the sample:

- Connects using the `Connect` function
- All operations are performed within the `Invoke-DataverseCommands` function

### Section 1: Basic Create and Update operations

Operations:

- Create a contact record using the `New-Record` function
- Update the contact record using the `Update-Record` function
- Retrieve the contact record using the `Get-Record` function
- Update a single property of the contact record using the `Set-ColumnValue` function
- Retrieve a single property of the contact record using the `Get-ColumnValue` function

See [HTTP Web API Basic Operations Sample Section 1: Basic create and update operations](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-basic-operations-sample#section-1-basic-create-and-update-operations) for low-level details.

### Section 2: Create record associated to another

Operations: Associate a new record to an existing one using the `New-Record` function.

See [HTTP Web API Basic Operations Sample Section 2: Create with association](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-basic-operations-sample#section-2-create-with-association) for low-level details.

### Section 3: Create related entities

Operations:

- Create the following records in one operation using the `New-Record` function.:

   - An account
   - A contact associated as the primary contact for the account record
   - 3 open tasks for that contact.  

   These records have the following relationships:

   ```
   Accounts
      |---[Primary] Contact (N-to-1)
         |---Tasks (1-to-N)
   ```

- Retrieve the related records using the `Get-Record` function

See [HTTP Web API Basic Operations Sample Section 3: Create related table rows (deep insert)](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-basic-operations-sample#section-3-create-related-table-rows-deep-insert) for low-level details.

### Section 4: Associate and Disassociate entities

Operations:

- Add a contact to the account `contact_customer_accounts` collection using the `Add-ToCollection` function
- Remove a contact from the account `contact_customer_accounts` collection using the `Remove-FromCollection` function
- Associate a security role to a user using the `systemuserroles_association` collection using the `Add-ToCollection` function
- Remove a security role for a user using the `systemuserroles_association` collection using the `Remove-FromCollection` function

See [HTTP Web API Basic Operations Sample Section 4: Associate and disassociate existing entities](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-basic-operations-sample#section-4-associate-and-disassociate-existing-entities) for low-level details.

### Section 5: Delete sample entities

Operations: A reference to each record created in this sample was added to a list as it was created.

When the `$deleteCreatedRecords` variable to `$true`, this section loops through that list and deletes each record using the `Remove-Record` function.

See [HTTP Web API Basic Operations Sample Section 5: Delete table rows](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-basic-operations-sample#section-5-delete-table-rows) for low-level details.

## Clean up

By default, this sample deletes all the records it created. If you want to view created records after the sample is complete, change the `$deleteCreatedRecords` variable to `false`.
