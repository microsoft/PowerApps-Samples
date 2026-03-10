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

This sample uses the common helper libraries created as described in [Use PowerShell and Visual Studio Code with the Dataverse Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/use-ps-and-vscode-web-api). These functions are defined in the [MetadataOperations.ps1](../MetadataOperations.ps1) file. You can find descriptions and examples in [Dataverse Web API PowerShell Helper Metadata operations](../README.md#metadata-operations-functions)

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
1. Open the [MetadataOperationsSample.ps1](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/PS/MetadataOperations/MetadataOperationsSample.ps1) file using Visual Studio Code
1. Edit this line to use the URL of the environment you want to connect to:

   `Connect 'https://yourorg.crm.dynamics.com/' # change this`

1. (Optional) Set the `$deleteCreatedRecords` variable to `$false` if you do not want to delete the records this sample creates.
1. Press F5 to run the sample.
1. The first time you run the sample a browser window opens. In the browser window, enter or select the credentials you want to use to authenticate.

To connect as a different user, run the [Disconnect-AzAccount command](https://learn.microsoft.com/powershell/module/az.accounts/disconnect-azaccount).

## Demonstrates

This sample has 11 sections:

1. [Create Publisher and Solution](#section-0-create-publisher-and-solution)
1. [Create, Retrieve and Update Table](#section-1-create-retrieve-and-update-table)
1. [Create, Retrieve and Update Columns](#section-2-create-retrieve-and-update-columns)
1. [Create and use Global OptionSet](#section-3-create-and-use-global-optionset)
1. [Create Customer Relationship](#section-4-create-customer-relationship)
1. [Create and retrieve a one-to-many relationship](#section-5-create-and-retrieve-a-one-to-many-relationship)
1. [Create and retrieve a many-to-one relationship](#section-6-create-and-retrieve-a-many-to-one-relationship)
1. [Create and retrieve a many-to-many relationship](#section-7-create-and-retrieve-a-many-to-many-relationship)
1. [Export managed solution](#section-8-export-managed-solution)
1. [Delete sample records](#section-9-delete-sample-records)
1. [Import and Delete managed solution](#section-10-import-and-delete-managed-solution)

### Section 0: Create Publisher and Solution

Operations: Create a solution record and an associated publisher record.

- After first checking whether these records already exist using the [Get-Records function](../README.md#get-records-function), the script creates them if they don't exist using the [New-Record function](../README.md#new-record-function). These functions are in the [TableOperations.ps1](../TableOperations.ps1) file.
- All solution components created in this sample get associated to the solution, so they can be exported. This association is created using the `MSCRM.SolutionUniqueName` request header setting the solution unique name set as the value.
- All names of solution components are prefixed using the publisher customization prefix.
- Details about these records are added to the `$recordsToDelete` array so that they can be deleted later.

### Section 1: Create, Retrieve and Update Table

Operations:

- Check whether a `sample_BankAccount` table exists using the [Get-Tables function](../README.md#get-tables-function).
- If the column doesn't exist, create a new `sample_BankAccount` user-owned table using the [New-Table function](../README.md#new-table-function).
- Retrieve the created table using the [Get-Table function](../README.md#get-table-function).
- Update the table using the [Update-Table function](../README.md#update-table-function).

### Section 2: Create, Retrieve and Update Columns

Operations:

- Check whether a boolean column named `sample_Boolean` exists in the `sample_BankAccount` table using the [Get-TableColumns function](../README.md#get-tablecolumns-function).
- If the column doesn't exist, create a new boolean column for the `sample_BankAccount` table using the [New-Column function](../README.md#new-column-function).
- Retrieve the boolean column using the [Get-Column function](../README.md#get-column-function).
- Update the boolean column using the the [Update-Column function](../README.md#update-column-function).
- Update the option labels for the boolean column using the [Update-OptionValue function](../README.md#update-optionvalue-function).
- Query, create, and retrieve a new datetime column for the `sample_BankAccount` table using the `Get-TableColumns`, `New-Column`, and `Get-Column` functions.
- Query, create, and retrieve a new decimal column for the `sample_BankAccount` table.
- Query, create, and retrieve a new integer column for the `sample_BankAccount` table.
- Query, create, and retrieve a new memo column for the `sample_BankAccount` table.
- Query, create, and retrieve a new money column for the `sample_BankAccount` table.
- Query, create, and retrieve a new choice (`Picklist`) column for the `sample_BankAccount` table.
  - Add a new option to the choice column using the [New-OptionValue function](../README.md#new-optionvalue-function)
  - Change the order of the options of the choice column using the [Update-OptionsOrder function](../README.md#update-optionsorder-function).
  - Delete one of the options of the choice column using the [Remove-OptionValue function](../README.md#remove-optionvalue-function).
- Query, create, and retrieve a new choices (`MultiSelectPicklist`) column for the `sample_BankAccount` table.
- Query, create, and retrieve a new big int column for the `sample_BankAccount` table.
- Create a new Status option for the `sample_BankAccount` table using the [New-StatusOption function](../README.md#new-statusoption-function).

### Section 3: Create and use Global OptionSet

Operations:

- Check whether global choice already exists using the [Get-GlobalOptionSet function](../README.md#get-globaloptionset-function).
- If it doesn't exist, create a new global choice using the [New-GlobalOptionSet function](../README.md#new-globaloptionset-function).
- Retrieve the global choice using the [Get-GlobalOptionSet function](../README.md#get-globaloptionset-function)
- Create a new choice column for the `sample_BankAccount` table using the global choice using the `New-Column` function

### Section 4: Create Customer Relationship

Operations:

- Check whether a customer column exists using the `Get-TableColumns` function.
- If it doesn't exist, create a new customer column for the `sample_BankAccount` table using the [New-CustomerRelationship function](../README.md#new-customerrelationship-function).
- Retrieve the customer column using the `Get-Column` function
- Retrieve the relationships created for the customer column using the [Get-Relationship function](../README.md#get-relationship-function).

### Section 5: Create and retrieve a one-to-many relationship

Operations:

- Verify that the `sample_BankAccount` table is eligible to be referenced in a 1:N relationship using the [Get-CanBeReferenced function](../README.md#get-canbereferenced-function).
- Verify that the `contact` table is eligible to be reference other tables in a 1:N relationship using the [Get-CanBeReferencing function](../README.md#get-canbereferencing-function).
- Identify what other tables can reference the `sample_BankAccount` table in a 1:N relationship using the [Get-ValidReferencingTables function](../README.md#get-validreferencingtables-function).
- Check whether a 1:N relationship exists using the [Get-Relationships function](../README.md#get-relationships-function).
- If it doesn't exist, create a 1:N relationship between `sample_BankAccount` and `contact` tables using the [New-Relationship function](../README.md#new-relationship-function).

### Section 6: Create and retrieve a many-to-one relationship

Operations:

- Create a N:1 relationship between `sample_BankAccount` and `account` tables using the [New-Relationship function](../README.md#new-relationship-function).
- Retrieve the N:1 relationship using the [Get-Relationships function](../README.md#get-relationships-function)

### Section 7: Create and retrieve a many-to-many relationship

Operations:

- Verify that the `sample_BankAccount` and `contact` tables are eligible to participate in a N:N relationship using the [Get-CanManyToMany function](../README.md#get-canmanytomany-function)
- Verify that the `sample_BankAccount` and `contact` tables are eligible to participate in a N:N relationship using the [Get-ValidManyToManyTables function](../README.md#get-validmanytomanytables-function)
- Create a N:N relationship between `sample_BankAccount` and `contact` tables using the [New-Relationship function](../README.md#new-relationship-function)
- Retrieve the N:N relationship using the [Get-Relationships function](../README.md#get-relationships-function)

### Section 8: Export managed solution

Operations: Export the solution containing the items created in this sample using the [Export-Solution function](../README.md#export-solution-function)

### Section 9: Delete sample records

Operations: A reference to each record created in this sample was added to a list as it was created. In this sample the records are deleted in the reverse order they were created using the [Remove-Record function](../README.md#remove-record-function)

### Section 10: Import and Delete managed solution

Operations:

- Import the solution exported in [Section 8](#section-8-export-managed-solution) using the [Import-Solution function](../README.md#import-solution-function)
- Query the solution table to get the id of the imported solution
- Delete the imported solution.

## Clean up

By default, this sample deletes all the records it created.

If you want to view created records, after the sample is completed, change the `deleteCreatedRecords` variable to `false` and you're prompted to delete the records if desired.

> [!NOTE]
> If you don't delete the unmanaged solution components created by this sample, the code in [Section 10](#section-10-import-and-delete-managed-solution) fails.

## Console Output

The complete output to the console should look something like this:

```text
PS C:\GitHub\PowerApps-Samples\dataverse\webapi\PS>
PS C:\GitHub\PowerApps-Samples\dataverse\webapi\PS> . 'C:\GitHub\PowerApps-Samples\dataverse\webapi\PS\MetadataOperations\MetadataOperationsSample.ps1'
Example Publisher created successfully
Metadata Example Solution created successfully
Bank Account table created successfully
Retrieved Bank Account table.
Bank Account table updated successfully
Sample Boolean column created successfully
Original Option Labels:
 True Option Label: True
 False Option Label: False
Sample Boolean Column updated successfully
Option values updated successfully
Updated Option Labels:
 True Option Label: Up
 False Option Label: Down
Example DateTime column created successfully
Retrieved Sample DateTime column.
 DateTimeBehavior: DateOnly
 Format: DateOnly
Sample Decimal column created successfully
Retrieved Sample Decimal column.
 MaxValue: 100
 MinValue: 0
 Precision: 1
Sample Integer column created successfully
Retrieved Sample Integer column.
 MaxValue: 100
 MinValue: 0
 Format: None
Sample Memo column created successfully
Retrieved Sample Memo column.
 Format: Text
 ImeMode: Disabled
 MaxLength: 500
Sample Money column created successfully
Retrieved Sample Money column.
 MaxValue: 1000
 MinValue: 0
 Precision: 1
 PrecisionSource: 1
 ImeMode: Disabled
Sample Choice column created successfully
Retrieved Sample Choice column.
Retrieved Choice column options:
 Value: 727000000 Label: Bravo
 Value: 727000001 Label: Delta
 Value: 727000002 Label: Alpha
 Value: 727000003 Label: Charlie
 Value: 727000004 Label: Foxtrot
Echo option added to the local optionset.
Retrieved Sample Choice column again.
Retrieved Choice column options:
 Value: 727000000 Label: Bravo
 Value: 727000001 Label: Delta
 Value: 727000002 Label: Alpha
 Value: 727000003 Label: Charlie
 Value: 727000004 Label: Foxtrot
 Value: 727000005 Label: Echo
Choice column options re-ordered.
Retrieved Sample Choice column again.
Retrieved Choice column options with new order:
 Value: 727000002 Label: Alpha
 Value: 727000000 Label: Bravo
 Value: 727000003 Label: Charlie
 Value: 727000001 Label: Delta
 Value: 727000005 Label: Echo
 Value: 727000004 Label: Foxtrot
Foxtrot option deleted from the local optionset.
Sample MultiSelect Choice column created successfully
Retrieved Sample MultiSelect Choice column.
Retrieved MultiSelect Choice column options:
 Value: 727000000 Label: Appetizer
 Value: 727000001 Label: Entree
 Value: 727000002 Label: Dessert
Sample BigInt column created successfully
Retrieved Sample BigInt column.
 MaxValue: 9223372036854775807
 MinValue: -9223372036854775808
Frozen status added to the status column.
With the value of: 727000000
Retrieved Status Reason column again.
Retrieved status column options:
 Value: 1 Label: Active State: 0
 Value: 2 Label: Inactive State: 1
 Value: 727000000 Label: Frozen State: 1
Colors global optionset created successfully                                                                
Retrieved Colors global optionset.
Retrieved global optionset options:
 Value: 727000000 Label: Red
 Value: 727000001 Label: Yellow
 Value: 727000002 Label: Green
Example Colors Choice column created successfully
Retrieved Sample Colors Choice column.
Retrieved Choice column options:
 Value: 727000000 Label: Red
 Value: 727000001 Label: Yellow
 Value: 727000002 Label: Green
Customer relationship created successfully
Example Customer Lookup column created successfully
Retrieved Sample Bank Account owner column Targets:
 account
 contact
Retrieved Customer relationship IDs:
 sample_BankAccount_Customer_Account
 sample_BankAccount_Customer_Contact
The Bank Account table is eligible to be a primary table in a one-to-many relationship.
The Bank Account table is eligible to be a related table in a one-to-many relationship.
The contact table is in the list of potential referencing entities for the Bank Account table.
sample_BankAccount_Contacts One-to-Many relationship created successfully
sample_Account_BankAccounts Many-to-One relationship created successfully
The contact table can participate in many-to-many relationships.
The Bank Account table can participate in many-to-many relationships.
The contact table is in the list of tables that can participate in many-to-many relationships
The Bank Account table is in the list of tables that can participate in many-to-many relationships
sample_sample_BankAccounts_Contacts Many-to-Many relationship created successfully
Managed solution exported to C:\GitHub\PowerApps-Samples\dataverse\webapi\PS\MetadataOperations\metadataexamplesolution.zip
Deleting sample records...
RelationshipDefinitions record with ID: 7746b5ec-7c11-ef11-9f89-7c1e520b124e deleted.
RelationshipDefinitions record with ID: 037f08e2-7c11-ef11-9f89-6045bdec757e deleted.
RelationshipDefinitions record with ID: db9953d9-7c11-ef11-9f89-7c1e520b124e deleted.
EntityDefinitions(LogicalName='sample_bankaccount')/Attributes record with ID: a49d9074-b9bc-425d-9ca6-8b846269316c deleted.
EntityDefinitions(LogicalName='sample_bankaccount')/Attributes record with ID: 5302e1c4-7c11-ef11-9f89-6045bdec757e deleted.
GlobalOptionSetDefinitions record with ID: 7c2d43c3-7c11-ef11-9f89-7c1e520b124e deleted.
EntityDefinitions(LogicalName='sample_bankaccount')/Attributes record with ID: cd48ebbf-7c11-ef11-9f89-6045bdec7f44 deleted.
EntityDefinitions(LogicalName='sample_bankaccount')/Attributes record with ID: 201015b8-7c11-ef11-9f89-6045bdec757e deleted.
EntityDefinitions(LogicalName='sample_bankaccount')/Attributes record with ID: 496638ab-7c11-ef11-9f89-6045bdec757e deleted.
EntityDefinitions(LogicalName='sample_bankaccount')/Attributes record with ID: 873943a2-7c11-ef11-9f89-7c1e520b124e deleted.
EntityDefinitions(LogicalName='sample_bankaccount')/Attributes record with ID: 554ba99e-7c11-ef11-9f89-6045bdec7f44 deleted.
EntityDefinitions(LogicalName='sample_bankaccount')/Attributes record with ID: 7479559c-7c11-ef11-9f89-7c1e5214ffc0 deleted.
EntityDefinitions(LogicalName='sample_bankaccount')/Attributes record with ID: 2ae45795-7c11-ef11-9f89-7c1e520b124e deleted.
EntityDefinitions(LogicalName='sample_bankaccount')/Attributes record with ID: c2c38696-7c11-ef11-9f89-6045bdec757e deleted.
EntityDefinitions(LogicalName='sample_bankaccount')/Attributes record with ID: c5437c8b-7c11-ef11-9f89-6045bdec7f44 deleted.
EntityDefinitions record with ID: 441f9871-7c11-ef11-9f89-6045bdec7f44 deleted.
solutions record with ID: 411f9871-7c11-ef11-9f89-6045bdec7f44 deleted.
publishers record with ID: 3e1f9871-7c11-ef11-9f89-6045bdec7f44 deleted.
Importing managed solution...

Managed solution imported.
Managed solution deleted.
PS C:\GitHub\PowerApps-Samples\dataverse\webapi\PS>
```
