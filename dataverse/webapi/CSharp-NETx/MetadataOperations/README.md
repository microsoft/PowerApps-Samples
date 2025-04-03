---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to work with schema definitions using the Dataverse Web API."
---

# Web API Metadata operations sample

This .NET 6.0 sample demonstrates how to work with these components using the Dataverse Web API:

- Table definitions
- Column definitions
- Option set definitions
- Relationship definitions
- Importing and exporting solutions

This sample uses the common helper code in the [WebAPIService](../WebAPIService) class library project.

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with system administrator privileges

## How to run the sample

1. Clone or download the [PowerApps-Samples](../../../../../PowerApps-Samples) repository.
1. Open the [MetadataOperations.sln](MetadataOperations.sln) file using Visual Studio 2022.
1. Edit the [appsettings.json](../appsettings.json) file to set the following property values.

   | Property | Instructions |
   |----------|--------------|
   | `Url` | The URL for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. Learn how to find your URL in [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources). |
   | `UserPrincipalName` | Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access the environment. |
   | `Password` | Replace the placeholder `yourPassword` value with the password you use. |

1. Save the `appsettings.json` file
1. Press `F5` to run the sample.

## Demonstrates

This sample has 11 sections:

1. [Create publisher and solution](#section-1-create-publisher-and-solution)
1. [Create, retrieve and update table](#section-2-create-retrieve-and-update-table)
1. [Create, retrieve and update columns](#section-3-create-retrieve-and-update-columns)
1. [Create and use global OptionSet](#section-4-create-and-use-global-optionset)
1. [Create customer relationship](#section-5-create-customer-relationship)
1. [Create and retrieve a one-to-many relationship](#section-6-create-and-retrieve-a-one-to-many-relationship)
1. [Create and retrieve a many-to-one relationship](#section-7-create-and-retrieve-a-many-to-one-relationship)
1. [Create and retrieve a many-to-many relationship](#section-8-create-and-retrieve-a-many-to-many-relationship)
1. [Export managed solution](#section-9-export-managed-solution)
1. [Delete sample records](#section-10-delete-sample-records)
1. [Import and delete managed solution](#section-11-import-and-delete-managed-solution)

### Section 1: Create publisher and solution

Operations: Create a solution record and an associated publisher record.

- All solution components created in this sample are associated to the solution, so they can be exported. This association is created using the `MSCRM.SolutionUniqueName` request header that sets the solution's unique name as the value.
- All names of solution components are prefixed using the publisher customization prefix.

### Section 2: Create, retrieve and update table

Operations:

- Create a new `sample_BankAccount` user-owned table using WebAPIService [CreateEntityRequest](../WebAPIService/Metadata/Messages/CreateEntityRequest.cs) and [CreateEntityResponse](../WebAPIService/Metadata/Messages/CreateEntityResponse.cs) classes.
- Retrieve the created table using the WebAPIService [RetrieveEntityDefinitionRequest](../WebAPIService/Metadata/Messages/RetrieveEntityDefinitionRequest.cs) and [RetrieveEntityDefinitionResponse](../WebAPIService/Metadata/Messages/RetrieveEntityDefinitionResponse.cs) classes.
- Update the table using the WebAPIService [UpdateEntityRequest class](../WebAPIService/Metadata/Messages/UpdateEntityRequest.cs).

### Section 3: Create, retrieve and update columns

Operations:

- Create a new *boolean* column for the `sample_BankAccount` table using WebAPIService [CreateAttributeRequest](../WebAPIService/Metadata/Messages/CreateAttributeRequest.cs) and [CreateAttributeResponse](../WebAPIService/Metadata/Messages/CreateAttributeResponse.cs) classes.
- Retrieve the *boolean* column using the WebAPIService [RetrieveAttributeRequest](../WebAPIService/Metadata/Messages/RetrieveAttributeRequest.cs) and [RetrieveAttributeResponse&lt;T&gt;](../WebAPIService/Metadata/Messages/RetrieveAttributeResponse.cs) classes.
- Update the *boolean* column using the the WebAPIService [UpdateAttributeRequest class](../WebAPIService/Metadata/Messages/UpdateAttributeRequest.cs).
- Update the option labels for the *boolean* column using the WebAPIService [UpdateOptionValueRequest class](../WebAPIService/Metadata/Messages/UpdateOptionValueRequest.cs).
- Create and retrieve the following columns for the `sample_BankAccount` table:
  - datetime
  - decimal
  - integer
  - memo
  - money
  - choice
  - choices
  - big int
- Add a new option to the *choice* column using the WebAPIService [InsertOptionValueRequest class](../WebAPIService/Metadata/Messages/InsertOptionValueRequest.cs).
- Change the order of the options of the *choice* column using the WebAPIService [OrderOptionRequest class](../WebAPIService/Metadata/Messages/OrderOptionRequest.cs).
- Delete one of the options of the *choice* column using the WebAPIService [DeleteOptionValueRequest class](../WebAPIService/Metadata/Messages/DeleteOptionValueRequest.cs).
- Create and retrieve a new multi-select *choice* column for the `sample_BankAccount` table.
- Create a new *status* option for the `sample_BankAccount` table using the WebAPIService [InsertStatusValueRequest](../WebAPIService/Metadata/Messages/InsertStatusValueRequest.cs) and [InsertStatusValueResponse](../WebAPIService/Metadata/Messages/InsertStatusValueResponse.cs) classes.

### Section 4: Create and use global OptionSet

Operations:

- Create a new global choice using the WebAPIService [CreateGlobalOptionSetRequest](../WebAPIService/Metadata/Messages/CreateGlobalOptionSetRequest.cs) and [CreateGlobalOptionSetResponse](../WebAPIService/Metadata/Messages/CreateGlobalOptionSetResponse.cs) classes.
- Retrieve the global choice using the WebAPIService [RetrieveGlobalOptionSetRequest](../WebAPIService/Metadata/Messages/RetrieveGlobalOptionSetRequest.cs) and [RetrieveGlobalOptionSetResponse](../WebAPIService/Metadata/Messages/RetrieveGlobalOptionSetResponse.cs) classes.
- Create a new *choice* column for the `sample_BankAccount` table using the global choice using the WebAPIService [CreateAttributeRequest](../WebAPIService/Metadata/Messages/CreateAttributeRequest.cs) and [CreateAttributeResponse](../WebAPIService/Metadata/Messages/CreateAttributeResponse.cs) classes.

### Section 5: Create customer relationship

Operations:

- Create a new *customer* column for the `sample_BankAccount` table using the WebAPIService [CreateCustomerRelationshipsRequest](../WebAPIService/Metadata/Messages/CreateCustomerRelationshipsRequest.cs) and [CreateCustomerRelationshipsResponse](../WebAPIService/Metadata/Messages/CreateCustomerRelationshipsResponse.cs) classes.
- Retrieve the *customer* column using the WebAPIService [RetrieveAttributeRequest](../WebAPIService/Metadata/Messages/RetrieveAttributeRequest.cs) and [RetrieveAttributeResponse&lt;T&gt;](../WebAPIService/Metadata/Messages/RetrieveAttributeResponse.cs) classes.
- Retrieve the relationships created for the *customer* column using the WebAPIService [RetrieveRelationshipRequest](../WebAPIService/Metadata/Messages/RetrieveRelationshipRequest.cs) and [RetrieveRelationshipResponse&lt;T&gt;](../WebAPIService/Metadata/Messages/RetrieveRelationshipResponse.cs) classes.

### Section 6: Create and retrieve a one-to-many relationship

Operations:

- Verify that the `sample_BankAccount` table is eligible to be referenced in a 1:N relationship using the WebAPIService [CanBeReferencedRequest](../WebAPIService/Metadata/Messages/CanBeReferencedRequest.cs) and [CanBeReferencedResponse](../WebAPIService/Metadata/Messages/CanBeReferencedResponse.cs) classes.
- Verify that the `contact` table is eligible to be reference other tables in a 1:N relationship using the WebAPIService [CanBeReferencingRequest](../WebAPIService/Metadata/Messages/CanBeReferencingRequest.cs) and [CanBeReferencingResponse](../WebAPIService/Metadata/Messages/CanBeReferencingResponse.cs) classes.
- Identify what other tables can reference the `sample_BankAccount` table in a 1:N relationship using the WebAPIService [GetValidReferencingEntitiesRequest](../WebAPIService/Metadata/Messages/GetValidReferencingEntitiesRequest.cs) and [GetValidReferencingEntitiesResponse](../WebAPIService/Metadata/Messages/GetValidReferencingEntitiesResponse.cs) classes.
- Create a 1:N relationship between `sample_BankAccount` and `contact` tables using the WebAPIService [CreateRelationshipRequest](../WebAPIService/Metadata/Messages/CreateRelationshipRequest.cs) and [CreateRelationshipResponse](../WebAPIService/Metadata/Messages/CreateRelationshipResponse.cs) classes.
- Retrieve the 1:N relationship using the WebAPIService [RetrieveRelationshipRequest](../WebAPIService/Metadata/Messages/RetrieveRelationshipRequest.cs) and [RetrieveRelationshipResponse&lt;T&gt;](../WebAPIService/Metadata/Messages/RetrieveRelationshipResponse.cs) classes.

### Section 7: Create and retrieve a many-to-one relationship

Operations:

- Create an N:1 relationship between `sample_BankAccount` and `account` tables using the WebAPIService [CreateRelationshipRequest](../WebAPIService/Metadata/Messages/CreateRelationshipRequest.cs) and [CreateRelationshipResponse](../WebAPIService/Metadata/Messages/CreateRelationshipResponse.cs) classes.
- Retrieve the N:1 relationship using the WebAPIService [RetrieveRelationshipRequest](../WebAPIService/Metadata/Messages/RetrieveRelationshipRequest.cs) and [RetrieveRelationshipResponse&lt;T&gt;](../WebAPIService/Metadata/Messages/RetrieveRelationshipResponse.cs) classes.

### Section 8: Create and retrieve a many-to-many relationship

Operations:

- Verify that the `sample_BankAccount` and `contact` tables are eligible to participate in an N:N relationship using the WebAPIService [CanManyToManyRequest](../WebAPIService/Metadata/Messages/CanManyToManyRequest.cs) and [CanManyToManyResponse](../WebAPIService/Metadata/Messages/CanManyToManyResponse.cs) classes.
- Verify that the `sample_BankAccount` and `contact` tables are eligible to participate in an N:N relationship using the WebAPIService [GetValidManyToManyRequest](../WebAPIService/Metadata/Messages/GetValidManyToManyRequest.cs) and [GetValidManyToManyResponse](../WebAPIService/Metadata/Messages/GetValidManyToManyResponse.cs) classes.
- Create an N:N relationship between `sample_BankAccount` and `contact` tables using the WebAPIService [CreateRelationshipRequest](../WebAPIService/Metadata/Messages/CreateRelationshipRequest.cs) and [CreateRelationshipResponse](../WebAPIService/Metadata/Messages/CreateRelationshipResponse.cs) classes.
- Retrieve the N:N relationship using the WebAPIService [RetrieveRelationshipRequest](../WebAPIService/Metadata/Messages/RetrieveRelationshipRequest.cs) and [RetrieveRelationshipResponse&lt;T&gt;](../WebAPIService/Metadata/Messages/RetrieveRelationshipResponse.cs) classes.

### Section 9: Export managed solution

Operations: Export the solution containing the items created in this sample using the WebAPIService [ExportSolutionRequest](../WebAPIService/Messages/ExportSolutionRequest.cs) and [ExportSolutionResponse](../WebAPIService/Messages/ExportSolutionResponse.cs) classes.

### Section 10: Delete sample records

Operations: A reference to each record created in this sample is added to a list as it's created. The records are deleted using a `$batch` operation using the WebAPIService [BatchRequest class](../WebAPIService/Batch/BatchRequest.cs).

### Section 11: Import and delete managed solution

Operations:

- Import the solution exported in [Section 9](#section-9-export-managed-solution) using the WebAPIService [ImportSolutionRequest class](../WebAPIService/Messages/ImportSolutionRequest.cs).
- Query the solution table to get the `id` of the imported solution.
- Delete the imported solution.

## Clean up

By default, this sample deletes all the records created in it.

If you want to view created records, after the sample is completed, change the `deleteCreatedRecords` variable to `false` and you're prompted to delete the records.

> [!NOTE]
> If you don't delete the unmanaged solution components created by this sample, the code in [Section 11](#section-11-import-and-delete-managed-solution) fails.
