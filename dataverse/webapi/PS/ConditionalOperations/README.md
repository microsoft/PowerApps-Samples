---
languages:
- powershell
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to perform conditional operations using ETags with the Dataverse Web API using PowerShell with Visual Studio Code."
---
# Dataverse Web API PowerShell Conditional Operations sample

This PowerShell version 7.4.0 sample demonstrates how to perform conditional operations using ETags with the Dataverse Web API and Visual Studio Code.

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
- [Perform conditional operations using the Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/perform-conditional-operations-using-web-api)

This sample requires:

- Visual Studio Code. See [Download Visual Studio Code](https://code.visualstudio.com/download)
- PowerShell extension for Visual Studio Code. See [PowerShell for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell)
- PowerShell 7.4 or higher. See [Install PowerShell on Windows, Linux, and macOS](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
- Az PowerShell module version 11.1.0 or higher. See [How to install Azure PowerShell](https://learn.microsoft.com/powershell/azure/install-azure-powershell)

   [To update an existing installation to the latest version](https://learn.microsoft.com/powershell/module/powershellget/update-module), use `Update-Module -Name Az -Force`

- Access to Dataverse with privileges to perform data operations.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the [ConditionalOperations.ps1](ConditionalOperations.ps1) file using Visual Studio Code
1. Edit this line to use the URL of the environment you want to connect to:

   `Connect 'https://yourorg.crm.dynamics.com/' # change this`

1. (Optional) Set the `$deleteCreatedRecords` variable to `$false` if you do not want to delete the records this sample creates.
1. Press F5 to run the sample.
1. The first time you run the sample a browser window opens. In the browser window, enter or select the credentials you want to use to authenticate.

To connect as a different user, run the [Disconnect-AzAccount command](https://learn.microsoft.com/powershell/module/az.accounts/disconnect-azaccount).

## Demonstrates

This sample has 3 sections that demonstrate conditional operations using ETags as described in detail within the [Web API Conditional Operations Sample](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-conditional-operations-sample)

To begin, the sample:

- Connects using the `Connect` function
- All operations are performed within the `Invoke-DataverseCommands` function

### Section 0: Create sample records

Operations:

- Create an account record using the `Add-Record` function with the `Prefer: return=representation` header to retrieve the created record with its initial ETag value
- Store the initial ETag value for later use in optimistic concurrency operations

See [Web API Conditional Operations Sample Section 0: Create sample records](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-conditional-operations-sample#section-0-create-sample-records) for low-level details.

### Section 1: Conditional GET

Operations:

- Use the `Sync-Record` function to perform a conditional GET with the `If-None-Match` header
  - When the ETag matches (record unchanged), returns HTTP 304 Not Modified and the original record
  - When the ETag doesn't match (record changed), returns HTTP 200 OK with the updated record and new ETag
- Update the account's telephone1 field using the `Set-ColumnValue` function to change the record version
- Perform another conditional GET to retrieve the updated record with its new ETag value

The `Sync-Record` function demonstrates how to optimize bandwidth by avoiding unnecessary data transfer when a record hasn't changed on the server.

See [Web API Conditional Operations Sample Section 1: Conditional GET](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-conditional-operations-sample#section-1-conditional-get) for low-level details.

### Section 2: Optimistic concurrency on delete and update

Operations:

- Attempt to delete the account using the `Remove-Record` function with the original (outdated) ETag value
  - Returns HTTP 412 Precondition Failed because the record version has changed
  - Demonstrates how optimistic concurrency prevents accidental deletion of modified records
- Attempt to update the account using the `Update-Record` function with the original (outdated) ETag value
  - Returns HTTP 412 Precondition Failed because the record version has changed
  - Demonstrates how optimistic concurrency prevents overwriting changes made by others
- Successfully update the account using the current (valid) ETag value
  - Returns HTTP 204 No Content on success
  - Demonstrates the proper optimistic concurrency workflow
- Retrieve the final account state using the `Get-Record` function to verify the update succeeded

Both `Remove-Record` and `Update-Record` functions support an optional `eTagValue` parameter that sets the `If-Match` header for optimistic concurrency control.

See [Web API Conditional Operations Sample Section 2: Optimistic concurrency on delete and update](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-conditional-operations-sample#section-2-optimistic-concurrency-on-delete-and-update) for low-level details.

### Section 3: Delete sample records

Operations: A reference to each record created in this sample was added to a list as it was created.

When the `$deleteCreatedRecords` variable is set to `$true`, this section loops through that list and deletes each record using the `Remove-Record` function.

See [Web API Conditional Operations Sample Section 3: Delete sample records](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-conditional-operations-sample#section-3-delete-sample-records) for low-level details.

## Key Functions Used

This sample uses the following functions from [TableOperations.ps1](../TableOperations.ps1):

- **Add-Record**: Creates a record and returns it with the `Prefer: return=representation` header, including the ETag value
- **Sync-Record**: Performs conditional GET using `If-None-Match` header to retrieve a record only if it has changed
- **Update-Record**: Updates a record with optional `eTagValue` parameter for optimistic concurrency using the `If-Match` header
- **Remove-Record**: Deletes a record with optional `eTagValue` parameter for optimistic concurrency using the `If-Match` header
- **Get-Record**: Retrieves a record with its current state and ETag value
- **Set-ColumnValue**: Updates a single property value of a record

## Understanding ETags

An ETag (entity tag) is a version identifier returned by the server for each record. ETags enable:

- **Conditional retrievals**: Avoid transferring data when the client already has the current version
- **Optimistic concurrency**: Prevent conflicting updates by verifying the record hasn't changed before modifying it

ETag values are returned in the `@odata.etag` property and typically look like: `W/"12345678"`

## Clean up

By default, this sample deletes all the records it created. If you want to view created records after the sample is complete, change the `$deleteCreatedRecords` variable to `$false`.
