---
languages:
- powershell
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to work with schema definitions using the Dataverse Web API using PowerShell."
---
# Web API PowerShell Metadata operations sample

This PowerShell sample demonstrates how to work with the following using the Dataverse Web API:

- Table definitions
- Column definitions
- Option set definitions
- Relationship definitions
- Importing and exporting solutions

This sample uses the common helper libraries created as described in [Use PowerShell and Visual Studio Code with the Dataverse Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/use-ps-and-vscode-web-api). These functions are defined in the [MetadataOperations.ps1](../MetadataOperations.ps1) file.

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

1. Clone or download the [PowerApps-Samples](../../../../../PowerApps-Samples) repository.
1. Open the [MetadataOperations.ps1](MetadataOperations.ps1) file using Visual Studio Code
1. Edit this line to use the URL of the environment you want to connect to:

   `Connect 'https://yourorg.crm.dynamics.com/' # change this`

1. (Optional) Set the `$deleteCreatedRecords` variable to `$false` if you do not want to delete the records this sample creates.
1. Press F5 to run the sample.
1. The first time you run the sample a browser window opens. In the browser window, enter or select the credentials you want to use to authenticate.

To connect as a different user, run the [Disconnect-AzAccount command](https://learn.microsoft.com/powershell/module/az.accounts/disconnect-azaccount).

## Demonstrates

This sample has 11 regions:

### Section 0: Create Publisher and Solution

Operations: Create a solution record and an associated publisher record.

- After first checking whether these records already exist using the `Get-Records` function, the script creates them if they don't exist using the `New-Record` function. These functions are in the [TableOperations.ps1](../TableOperations.ps1) file.
- All solution components created in this sample will be associated to the solution so that they can be exported. This association is created using the `MSCRM.SolutionUniqueName` request header setting the solution unique name set as the value.
- All names of solution components are prefixed using the publisher customization prefix.
- Details about these records are added to the `$recordsToDelete` array so that they can be deleted later.

### Section 1: Create, Retrieve and Update Table

Operations:

- Check whether a `sample_BankAccount` table exists using the `Get-Tables` function.
- If the column doesn't exist, create a new `sample_BankAccount` user-owned table using the `New-Table` function.
- Retrieve the created table using the `Get-Table` function.
- Update the table using the `Update-Table` function.

### Section 2: Create, Retrieve and Update Columns

Operations:

- Check whether a boolean column named `sample_Boolean` exists in the `sample_BankAccount` table using the `Get-TableColumns` function.
- If the column doesn't exist, create a new boolean column for the `sample_BankAccount` table using the `New-Column` function.
- Retrieve the boolean column using the `Get-Column` function.
- Update the boolean column using the the `Update-Column` function.
- Update the option labels for the boolean column using the `Update-OptionValue` function.
- Query, create, and retrieve a new datetime column for the `sample_BankAccount` table using the `Get-TableColumns`, `New-Column`, and `Get-Column` functions.
- Query, create, and retrieve a new decimal column for the `sample_BankAccount` table.
- Query, create, and retrieve a new integer column for the `sample_BankAccount` table.
- Query, create, and retrieve a new memo column for the `sample_BankAccount` table.
- Query, create, and retrieve a new money column for the `sample_BankAccount` table.
- Query, create, and retrieve a new choice (`Picklist`) column for the `sample_BankAccount` table.
   - Add a new option to the choice column using the `New-OptionValue` function
   - Change the order of the options of the choice column using the `Update-OptionsOrder` function.
   - Delete one of the options of the choice column using the `Remove-OptionValue` function.
- Query, create, and retrieve a new choices (`MultiSelectPicklist`) column for the `sample_BankAccount` table.
- Query, create, and retrieve a new big int column for the `sample_BankAccount` table.
- Create a new Status option for the `sample_BankAccount` table using the `New-StatusOption` function.

### Section 3: Create and use Global OptionSet

Operations:

- Check whether global choice already exists using the `Get-GlobalOptionSet` function.
- If it doesn't exist, create a new global choice using the `New-GlobalOptionSet` function.
- Retrieve the global choice using the `Get-GlobalOptionSet` function
- Create a new choice column for the `sample_BankAccount` table using the global choice using the `New-Column` function

### Section 4: Create Customer Relationship

Operations:

- Check whether a customer column exists using the `Get-TableColumns` function.
- If it doesn't exist, create a new customer column for the `sample_BankAccount` table using the `New-CustomerRelationship` function.
- Retrieve the customer column using the `Get-Column` function
- Retrieve the relationships created for the customer column using the `Get-Relationship` function.

### Section 5: Create and retrieve a one-to-many relationship

Operations:

- Verify that the `sample_BankAccount` table is eligible to be referenced in a 1:N relationship using the `Get-CanBeReferenced`  function.
- Verify that the `contact` table is eligible to be reference other tables in a 1:N relationship using the `Get-CanBeReferencing` function.
- Identify what other tables can reference the `sample_BankAccount` table in a 1:N relationship using the `Get-ValidReferencingTables` function.
- Check whether a 1:N relationship exists using the `Get-Relationships` function.
- If it doesn't exist, create a 1:N relationship between `sample_BankAccount` and `contact` tables using the `New-Relationship` function.

### Section 6: Create and retrieve a many-to-one relationship

Operations:

- Create a N:1 relationship between `sample_BankAccount` and `account` tables using the TODO.
- Retrieve the N:1 relationship using the TODO

### Section 7: Create and retrieve a many-to-many relationship

Operations:

- Verify that the `sample_BankAccount` and `contact` tables are eligible to participate in a N:N relationship using the TODO
- Verify that the `sample_BankAccount` and `contact` tables are eligible to participate in a N:N relationship using the TODO
- Create a N:N relationship between `sample_BankAccount` and `contact` tables using the TODO
- Retrieve the N:N relationship using the TODO

### Section 8: Export managed solution

Operations: Export the solution containing the items created in this sample using the TODO

### Section 9: Delete sample records

Operations: A reference to each record created in this sample was added to a list as it was created. In this sample the records are deleted using a `$batch` operation using the TODO

### Section 10: Import and Delete managed solution

Operations:

- Import the solution exported in [Section 8](#section-8-export-managed-solution) using the TODO
- Query the solution table to get the id of the imported solution
- Delete the imported solution.

## Clean up

By default this sample will delete all the records created in it.

If you want to view created records after the sample is completed, change the `deleteCreatedRecords` variable to `false` and you will be prompted to decide if you want to delete the records.

**Note**: If you do not delete the un-managed solution components created by this sample, the code in [Section 10](#section-10-import-and-delete-managed-solution) will fail.
