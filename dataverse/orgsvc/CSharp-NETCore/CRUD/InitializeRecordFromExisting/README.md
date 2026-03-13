# Initialize a record from existing record

This sample shows how to use the [InitializeFromRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializefromrequest) message to create new records initialized from existing records.

## How to run this sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
2. Navigate to the `dataverse/orgsvc/CSharp-NETCore/CRUD/` folder.
3. Edit the `appsettings.json` file to set your Dataverse connection string.
4. Build and run the sample:

```bash
cd InitializeRecordFromExisting
dotnet build
dotnet run
```

## What this sample does

The `InitializeFromRequest` message is used in scenarios where you need to create a new record initialized with values from an existing record. This is commonly used when:

- Creating a new account based on an existing account template
- Converting a lead to an opportunity while preserving relevant data
- Duplicating records with appropriate field mappings

This sample demonstrates two use cases:

1. **Account to Account**: Initializing a new account from an existing account
2. **Lead to Opportunity**: Initializing an opportunity from a lead (common sales scenario)

## How this sample works

The sample follows the Setup/Run/Cleanup pattern common to modern .NET 6+ samples:

### Setup

1. Creates an initial Account record named "Contoso, Ltd"
2. Creates an initial Lead record with subject "A Sample Lead" for contact "Colin Wilcox"
3. Stores references to these entities in the entityStore dictionary for later cleanup

### Demonstrate (Run)

1. **Initialize Account from Account**:
   - Creates an `InitializeFromRequest` with `TargetEntityName = "account"`
   - Sets the `EntityMoniker` to reference the existing account
   - Executes the request to get an initialized account entity
   - The initialized account inherits mapped fields from the source account

2. **Initialize Opportunity from Lead**:
   - Creates an `InitializeFromRequest` with `TargetEntityName = "opportunity"`
   - Sets the `EntityMoniker` to reference the existing lead
   - Executes the request to get an initialized opportunity entity
   - The initialized opportunity inherits relevant fields from the lead (like contact information)

### Cleanup

Prompts the user before deleting all created records (account and lead) from Dataverse.

## Key concepts

- **InitializeFromRequest**: Message that initializes a new entity from an existing entity based on entity mapping configuration
- **EntityMoniker**: Reference to the source entity from which to initialize
- **TargetEntityName**: The logical name of the entity type to create
- **Entity Mappings**: Dataverse uses predefined entity mappings to determine which fields to copy from source to target

## See also

- [InitializeFromRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializefromrequest)
- [Entity mapping and attribute mapping](https://learn.microsoft.com/power-apps/developer/data-platform/customize-entity-attribute-mappings)
- [Lead to opportunity conversion](https://learn.microsoft.com/dynamics365/sales/qualify-lead-convert-opportunity-sales)
