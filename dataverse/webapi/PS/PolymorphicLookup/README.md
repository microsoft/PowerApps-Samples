---
languages:
- powershell
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to create and use polymorphic lookup columns using the Dataverse Web API using PowerShell."
---
# Web API PowerShell Polymorphic Lookup sample

This PowerShell sample demonstrates how to work with polymorphic lookup columns (also known as multi-table lookups) using the Dataverse Web API:

- Creating multiple tables that can be referenced by a single lookup column
- Creating a polymorphic lookup attribute using the `CreatePolymorphicLookupAttribute` action
- Creating records and associating them through the polymorphic lookup
- Retrieving records with polymorphic lookup values and identifying referenced entity types
- Exporting and importing a managed solution containing the tables and relationships

This sample uses the common helper libraries created as described in [Use PowerShell and Visual Studio Code with the Dataverse Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/use-ps-and-vscode-web-api). These functions are defined in the [MetadataOperations.ps1](../MetadataOperations.ps1) file. You can find descriptions and examples in [Dataverse Web API PowerShell Helper Metadata operations](../README.md#metadata-operations-functions)

## Prerequisites

Before running this sample you should read these articles that explain concepts and patterns used by these samples:

- [Quick Start Web API with PowerShell and Visual Studio Code](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/quick-start-ps)
- [Use PowerShell and Visual Studio Code with the Dataverse Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/use-ps-and-vscode-web-api)
- [Multi-table (polymorphic) lookups](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/multitable-lookup)

This sample requires:

- Visual Studio Code. See [Download Visual Studio Code](https://code.visualstudio.com/download)
- PowerShell extension for Visual Studio Code. See [PowerShell for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell)
- PowerShell 7.4 or higher. See [Install PowerShell on Windows, Linux, and macOS](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
- Az PowerShell module version 11.1.0 or higher. See [How to install Azure PowerShell](https://learn.microsoft.com/powershell/azure/install-azure-powershell)

   [To update an existing installation to the latest version](https://learn.microsoft.com/powershell/module/powershellget/update-module), use `Update-Module -Name Az -Force`

- Access to Dataverse with privileges to perform data operations.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the [PolymorphicLookupSample.ps1](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/PS/PolymorphicLookup/PolymorphicLookupSample.ps1) file using Visual Studio Code
1. Edit this line to use the URL of the environment you want to connect to:

   `Connect 'https://yourorg.crm.dynamics.com/'`

1. (Optional) Set the `$deleteCreatedRecords` variable to `$false` if you do not want to delete the records this sample creates.
1. Press F5 to run the sample.
1. The first time you run the sample a browser window opens. In the browser window, enter or select the credentials you want to use to authenticate.

To connect as a different user, run the [Disconnect-AzAccount command](https://learn.microsoft.com/powershell/module/az.accounts/disconnect-azaccount).

## Demonstrates

This sample has 9 sections:

1. [Create Publisher and Solution](#section-0-create-publisher-and-solution)
1. [Create Referenced Tables](#section-1-create-referenced-tables)
1. [Create Referencing Table](#section-2-create-referencing-table)
1. [Create Polymorphic Lookup Attribute](#section-3-create-polymorphic-lookup-attribute)
1. [Create Sample Data Records](#section-4-create-sample-data-records)
1. [Retrieve Sample Data](#section-5-retrieve-sample-data)
1. [Export Managed Solution](#section-6-export-managed-solution)
1. [Delete Sample Tables and Solution](#section-7-delete-sample-tables-and-solution)
1. [Import and Delete Managed Solution](#section-8-import-and-delete-managed-solution)

### Section 0: Create Publisher and Solution

Operations: Create a solution record and an associated publisher record.

- After first checking whether these records already exist using the [Get-Records function](../README.md#get-records-function), the script creates them if they don't exist using the [New-Record function](../README.md#new-record-function). These functions are in the [TableOperations.ps1](../TableOperations.ps1) file.
- All solution components created in this sample get associated to the solution, so they can be exported. This association is created using the `MSCRM.SolutionUniqueName` request header setting the solution unique name set as the value.
- All names of solution components are prefixed using the publisher customization prefix.

### Section 1: Create Referenced Tables

Operations: Create three tables that a polymorphic lookup on `sample_Media` can reference.

- For each table (`sample_Book`, `sample_Audio`, `sample_Video`), the script first checks whether the table already exists using the [Get-Tables function](../README.md#get-tables-function).
- If it doesn't exist, each table is created using the [New-Table function](../README.md#new-table-function).
- Table labels are constructed using the [New-Label function](../README.md#new-label-function).
- The primary name attribute for each table is constructed using the [New-PrimaryNameAttribute function](../README.md#new-primarynameattribute-function).
- Each table includes a secondary string attribute specific to the media type: `sample_CallNumber` for books, `sample_AudioFormat` for audio, and `sample_VideoFormat` for video.

### Section 2: Create Referencing Table

Operations: Create the `sample_Media` table that will hold the polymorphic lookup column.

- The script first checks whether the table already exists using the [Get-Tables function](../README.md#get-tables-function).
- If it doesn't exist, the table is created using the [New-Table function](../README.md#new-table-function).
- Table labels are constructed using the [New-Label function](../README.md#new-label-function) and the primary name attribute using the [New-PrimaryNameAttribute function](../README.md#new-primarynameattribute-function).

### Section 3: Create Polymorphic Lookup Attribute

Operations: Create a single `sample_MediaPolymorphicLookup` lookup attribute on `sample_Media` that can reference records in `sample_Book`, `sample_Audio`, or `sample_Video`.

- The script first checks whether the lookup already exists by querying the `sample_media_sample_book` relationship using the [Get-Relationships function](../README.md#get-relationships-function).
- If the lookup doesn't yet exist, it is created using the [New-PolymorphicLookupColumn function](../README.md#new-polymorphiclookupcolumn-function), which calls the `CreatePolymorphicLookupAttribute` action. This creates one lookup attribute and three one-to-many relationships in a single request. The second and third relationships include a custom `CascadeConfiguration`.
- After creating the lookup, the `ReferencingEntityNavigationPropertyName` for each of the three relationships is retrieved using the [Get-Relationships function](../README.md#get-relationships-function). These navigation property names are required when setting the lookup value via `@odata.bind`.

### Section 4: Create Sample Data Records

Operations: Create records in each of the four tables and link `sample_Media` records to referenced records through the polymorphic lookup.

- Retrieves the entity set name for each table using the [Get-Table function](../README.md#get-table-function).
- Creates two `sample_Book` records, two `sample_Audio` records, and two `sample_Video` records using the [New-Record function](../README.md#new-record-function).
- Creates four `sample_Media` records, each linking to a different referenced record via the polymorphic lookup using the `@odata.bind` syntax with the navigation property name retrieved in Section 3.

### Section 5: Retrieve Sample Data

Operations: Query the `sample_Media` table to display each record's polymorphic lookup value and the type of the referenced entity.

- Retrieves all `sample_Media` records including the lookup value column using the [Get-Records function](../README.md#get-records-function).
- Uses OData annotations (`@OData.Community.Display.V1.FormattedValue` and `@Microsoft.Dynamics.CRM.lookuplogicalname`) to display the referenced record's display name and entity type.
- Demonstrates a cross-table query by separately retrieving `sample_Media` records that reference a specific `sample_Book` record and a specific `sample_Audio` record using the [Get-Records function](../README.md#get-records-function).

### Section 6: Export Managed Solution

Operations: Export the solution containing the items created in this sample using the [Export-Solution function](../README.md#export-solution-function).

### Section 7: Delete Sample Tables and Solution

Operations:

- Deletes the `sample_Media` table first, which cascades to also delete the polymorphic lookup attribute and its three relationships, as well as all `sample_Media` data records, using the [Remove-Record function](../README.md#remove-record-function).
- Deletes each referenced table (`sample_Book`, `sample_Audio`, `sample_Video`) using the [Remove-Record function](../README.md#remove-record-function). Deleting each table also removes all its data records.
- Retrieves and deletes the unmanaged solution using the [Get-Records function](../README.md#get-records-function) and the [Remove-Record function](../README.md#remove-record-function).

### Section 8: Import and Delete Managed Solution

Operations:

- Imports the solution exported in [Section 6](#section-6-export-managed-solution) using the [Import-Solution function](../README.md#import-solution-function).
- Queries the solution table to get the ID of the imported solution using the [Get-Records function](../README.md#get-records-function).
- Deletes the imported solution using the [Remove-Record function](../README.md#remove-record-function).

## Clean up

By default, this sample deletes all the records it creates.

If you want to view created records, after the sample is completed, change the `deleteCreatedRecords` variable to `false` and you're prompted to delete the records if desired.

> [!NOTE]
> If you don't delete the unmanaged solution components created by this sample, the code in [Section 8](#section-8-import-and-delete-managed-solution) fails.

## Console Output

The complete output to the console should look something like this:

```text
PS C:\GitHub\PowerApps-Samples\dataverse\webapi\PS>
PS C:\GitHub\PowerApps-Samples\dataverse\webapi\PS> . 'C:\GitHub\PowerApps-Samples\dataverse\webapi\PS\PolymorphicLookup\PolymorphicLookupSample.ps1'
Example Publisher created successfully
Polymorphic Lookup Example Solution created successfully
Book table created successfully
Audio table created successfully
Video table created successfully
Media table created successfully
Polymorphic lookup attribute 'sample_MediaPolymorphicLookup' created successfully
  Attribute ID: 611d231d-938a-46ec-ab95-ee8a2bf178fc
  Relationship IDs: ec8e9781-5117-f111-8342-0022482aa3a2, f48e9781-5117-f111-8342-0022482aa3a2, fc8e9781-5117-f111-8342-0022482aa3a2
Navigation property names:
  Book:  sample_MediaPolymorphicLookup_sample_book
  Audio: sample_MediaPolymorphicLookup_sample_audio
  Video: sample_MediaPolymorphicLookup_sample_video

Entity set names:
  Book:  sample_books
  Audio: sample_audios
  Video: sample_videos
  Media: sample_medias
Created Book record: Content1 (1ww-3452) - ID: 8b98de92-5117-f111-8341-7ced8d1dd398
Created Book record: Content2 (a4e-87hw) - ID: 145dfa93-5117-f111-8341-7ced8d21aac6
Created Audio record: Content1 (mp4) - ID: 8f4c5a95-5117-f111-8342-0022482aa3a2
Created Audio record: Content2 (wma) - ID: c5712b97-5117-f111-8341-0022482aa957
Created Video record: Content3 (wmv) - ID: 4b227d95-5117-f111-8341-0022482aa60e
Created Video record: Content2 (avi) - ID: 974c5a95-5117-f111-8342-0022482aa3a2
Created Media record: Media Object One -> Book:First Book - ID: e47bd998-5117-f111-8341-7ced8d1dd398
Created Media record: Media Object Two -> Audio:First Audio - ID: d1712b97-5117-f111-8341-0022482aa957
Created Media record: Media Object Three -> Video:First Video - ID: d3712b97-5117-f111-8341-0022482aa957
Created Media record: Media Object Four -> Audio:Second Audio - ID: 59227d95-5117-f111-8341-0022482aa60e

-- Retrieving Media records with polymorphic lookup values --

Media catalog entries:
  Media Object Four -> [sample_audio] Content2 (ID: c5712b97-5117-f111-8341-0022482aa957)
  Media Object Two -> [sample_audio] Content1 (ID: 8f4c5a95-5117-f111-8342-0022482aa3a2)
  Media Object Three -> [sample_video] Content3 (ID: 4b227d95-5117-f111-8341-0022482aa60e)
  Media Object One -> [sample_book] Content1 (ID: 8b98de92-5117-f111-8341-7ced8d1dd398)

Demonstrating cross-table lookup: querying Media records
  where the referenced item is named 'Content1'
  Media records referencing Book 'Content1':
    - Media Object One
  Media records referencing Audio 'Content1':
    - Media Object Two
Managed solution exported to C:\GitHub\PowerApps-Samples\dataverse\webapi\PS\PolymorphicLookup\polymorphiclookupexamplesolution.zip

Deleting sample tables and solution...
sample_Media table deleted.
sample_Book table deleted.
sample_Audio table deleted.
sample_Video table deleted.
Unmanaged solution 'polymorphiclookupexamplesolution' deleted.

Managed solution imported.
Managed solution deleted.

Sample completed in 00:19:36
```
