# Create and update records with related records

This sample demonstrates how to create and update a record and related records in one call using the following methods:

- [IOrganizationService.Create](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.create)
- [IOrganizationService.Update](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.update)

## Key Concepts

This sample shows how to use the `RelatedEntities` property to include related records in create and update operations. This allows you to:

1. Create a parent record along with multiple related child records in a single `Create` call
2. Update a parent record along with multiple related child records in a single `Update` call

## How to run this sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository
2. Navigate to the `dataverse/orgsvc/CSharp-NETCore/CRUD` directory
3. Update `appsettings.json` with your Dataverse environment connection string
4. Run the sample:
   ```bash
   cd CreateUpdateRecordsWithRelatedRecords
   dotnet run
   ```

## What this sample does

The sample demonstrates the following operations:

### Create Operation
1. Creates an account record named "Example Account"
2. Creates 3 letter activity records with subjects "Letter 1", "Letter 2", and "Letter 3"
3. Establishes the relationship between the account and letters
4. Creates all records in a single `Create` operation using the `RelatedEntities` collection

### Update Operation
1. Updates the account name to "Example Account - Updated"
2. Updates all 3 letter subjects to include "- Updated" suffix
3. Updates all records in a single `Update` operation using the `RelatedEntities` collection

## How this sample works

### Setup
No setup is required for this sample. All necessary records are created during the Run phase.

### Demonstrate

1. **Create account with related letters**:
   - Creates an `Entity` object for an account
   - Creates an `EntityCollection` containing 3 letter entities
   - Defines a `Relationship` object for "Account_Letters"
   - Adds the letters to the account's `RelatedEntities` using the relationship
   - Calls `service.Create()` to create all records in one operation

2. **Update account with related letters**:
   - Creates an `Entity` object for the account with updated name
   - Creates an `EntityCollection` containing the 3 existing letters with updated subjects
   - Adds the letters to the account's `RelatedEntities` using the same relationship
   - Calls `service.Update()` to update all records in one operation

### Cleanup
Displays an option to delete the records created. The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.

## Key Features

- **Late-bound Entity syntax**: Uses the `Entity` class with string-based attribute access
- **RelatedEntities collection**: Demonstrates using `Entity.RelatedEntities` to work with related records
- **Relationship objects**: Shows how to define relationships between entities
- **Single operation efficiency**: Creates/updates multiple related records in one call, reducing round trips

## Related Resources

- [Create related table rows in one operation](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-create#create-related-table-rows-in-one-operation)
- [Update related table rows in one operation](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-update#update-related-table-rows-in-one-operation)
- [Entity.RelatedEntities Property](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.entity.relatedentities)
- [Relationship Class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.relationship)
