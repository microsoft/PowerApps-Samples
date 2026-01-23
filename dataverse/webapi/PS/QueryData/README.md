---
languages:
- powershell
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to query Dataverse data using OData query syntax and FetchXML with the Web API."
---
# Web API PowerShell Query Data sample

This PowerShell sample demonstrates how to query Dataverse data using:

- OData query syntax ($select, $filter, $orderby, $top, $count, $expand, $apply)
- Query functions and operators
- Parameter aliases
- Pagination with nextLink
- Aggregation
- FetchXML queries
- Predefined queries (saved queries and user queries)

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
- [Query data using the Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/query-data-web-api)

This sample requires:

- Visual Studio Code. See [Download Visual Studio Code](https://code.visualstudio.com/download)
- PowerShell extension for Visual Studio Code. See [PowerShell for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell)
- PowerShell 7.4 or higher. See [Install PowerShell on Windows, Linux, and macOS](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
- Az PowerShell module version 11.1.0 or higher. See [How to install Azure PowerShell](https://learn.microsoft.com/powershell/azure/install-azure-powershell)

   [To update an existing installation to the latest version](https://learn.microsoft.com/powershell/module/powershellget/update-module), use `Update-Module -Name Az -Force`

- Access to Dataverse with privileges to perform data operations.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the [QueryData.ps1](QueryData.ps1) file using Visual Studio Code
1. Edit this line to use the URL of the environment you want to connect to:

   `Connect 'https://yourorg.crm.dynamics.com/' # change this`

1. (Optional) Set the `$deleteCreatedRecords` variable to `$false` if you do not want to delete the records this sample creates.
1. Press F5 to run the sample.
1. The first time you run the sample a browser window opens. In the browser window, enter or select the credentials you want to use to authenticate.

To connect as a different user, run the [Disconnect-AzAccount command](https://learn.microsoft.com/powershell/module/az.accounts/disconnect-azaccount).

## Demonstrates

This sample has 9 sections:

### Setup: Create records to query

Operations: Create sample data for the query demonstrations.

- Create one account record named 'Contoso, Ltd. (sample)' with the following related records:
  - A primary contact: Yvonne McKay
  - 8 related contacts in the `contact_customer_accounts` collection
  - 3 tasks for the account
  - 3 tasks for each contact (27 total tasks)

All records are added to the `$recordsToDelete` array for cleanup.

### Section 1: Select specific properties

Operations: Use `$select` to retrieve specific columns from a record.

- Retrieve `fullname`, `jobtitle`, and `annualincome` for a contact using the `Get-Record` function
- Display formatted values using OData annotations (e.g., `annualincome@OData.Community.Display.V1.FormattedValue`)

### Section 2: Use query functions

Operations: Use OData query functions and operators in `$filter` expressions.

- Filter records using `contains` function
- Filter records using `Microsoft.Dynamics.CRM.LastXHours` query function
- Use comparison operators (`gt` - greater than)
- Control filter precedence using parentheses with `and`/`or` operators

### Section 3: Ordering and aliases

Operations: Sort query results and use parameter aliases.

- Order results using `$orderby` with ascending and descending sort
- Use parameterized aliases (e.g., `@p1`, `@p2`) to make queries more maintainable

### Section 4: Limit and count results

Operations: Limit the number of results and get counts.

- Limit results using `$top`
- Get a count of records using `/$count`
- Include count with results using `$count=true`

### Section 5: Pagination

Operations: Retrieve results across multiple pages.

- Use `maxPageSize` parameter to control page size
- Use `@odata.nextLink` to retrieve subsequent pages using the `Get-NextLink` function

### Section 6: Expand results

Operations: Retrieve related records using `$expand`.

- Expand single-valued navigation properties (e.g., `primarycontactid`)
- Expand collection-valued navigation properties (e.g., `contact_customer_accounts`)
- Expand multiple navigation properties in a single request
- Perform nested expands of single-valued navigation properties
- Perform nested expands with both single-valued and collection-valued navigation properties

### Section 7: Aggregate results

Operations: Perform aggregations using `$apply`.

- Calculate aggregate values (average, sum, min, max) for numeric columns
- Display formatted aggregate values

### Section 8: FetchXML queries

Operations: Query data using FetchXML instead of OData.

- Execute basic FetchXML queries with filters and ordering
- Implement simple paging using `page` and `count` attributes
- Implement paging with paging cookies to handle large datasets:
  - Extract and decode the `@Microsoft.Dynamics.CRM.fetchxmlpagingcookie` annotation
  - Update FetchXML with the paging cookie for subsequent pages
  - Loop through all pages until `@Microsoft.Dynamics.CRM.morerecords` is false

### Section 9: Use predefined queries

Operations: Use saved queries and user queries.

- Retrieve and execute a system saved query (e.g., 'Active Accounts') using `savedQuery` parameter
- Create a user query (personal view) using the `New-Record` function
- Execute the user query using `userQuery` parameter

### Cleanup: Delete sample records

Operations: A reference to each record created in this sample was added to a list as it was created.

When the `$deleteCreatedRecords` variable is set to `$true`, this section loops through that list and deletes each record using the `Remove-Record` function.

## Clean up

By default, this sample deletes all the records it created. If you want to view created records after the sample is complete, change the `$deleteCreatedRecords` variable to `$false`.
