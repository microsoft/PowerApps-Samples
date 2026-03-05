# CRUD operations using dynamic (late-bound) entities

This sample demonstrates basic Create, Retrieve, Update, and Delete (CRUD) operations using dynamic (late-bound) entities in Microsoft Dataverse.

## What this sample does

This sample shows how to:

- **Create** an account entity using late-bound syntax
- **Retrieve** the account with specific columns
- **Update** the account with new attribute values
- **Delete** the account to clean up

The sample emphasizes the use of late-bound entities, where entity types and attributes are specified as strings rather than strongly-typed classes.

## How this sample works

### Setup

The Setup method initializes the entity store dictionary. This sample doesn't require any pre-existing data.

### Demonstrate

1. **Create**: Creates a new account entity using `Entity("account")` and sets attributes using string indexer syntax:
   - Sets the required `name` attribute to "Fourth Coffee"
   - Sets optional `address2_postalcode` to "98074"
   - Uses `service.Create()` to create the record in Dataverse

2. **Retrieve**: Retrieves the account using `service.Retrieve()`:
   - Uses `ColumnSet` to specify which attributes to retrieve (`name` and `ownerid`)
   - Demonstrates accessing retrieved attribute values with `GetAttributeValue<T>()`

3. **Update**: Updates the account using proper update pattern:
   - Creates a new `Entity` instance (does not reuse retrieved entity)
   - Sets the primary key (`accountid`) to match the entity to update
   - Updates `address1_postalcode` to "98052"
   - Clears `address2_postalcode` by setting it to null
   - Demonstrates using `Money` type for revenue
   - Demonstrates using boolean for `creditonhold`

4. **Delete**: Removes the account in the Cleanup method

### Key Concepts

**Late-bound vs Early-bound Entities:**
- **Late-bound** (this sample): Uses the generic `Entity` class with string-based entity names and attributes
  ```csharp
  Entity account = new Entity("account");
  account["name"] = "Fourth Coffee";
  ```
- **Early-bound**: Uses generated strongly-typed classes
  ```csharp
  Account account = new Account();
  account.Name = "Fourth Coffee";
  ```

**Important Update Pattern:**
> Do not update an entity using a retrieved entity instance. Always instantiate a new Entity and set the primary key value to match the entity you want to update. Only set the attribute values you are changing.

This prevents accidentally updating attributes that shouldn't be changed and improves performance.

### Clean up

The Cleanup method deletes the account entity that was created during the demonstration.

## Run this sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.

2. Navigate to the sample directory:
   ```
   cd dataverse/orgsvc/CSharp-NETCore/CRUD/CRUD-Dynamic-Entity
   ```

3. Edit the `appsettings.json` file in the parent CRUD directory (`dataverse/orgsvc/CSharp-NETCore/CRUD/appsettings.json`) with your Dataverse environment connection information:
   ```json
   {
     "ConnectionStrings": {
       "default": "AuthType=OAuth;Url=https://yourorg.crm.dynamics.com;Username=youruser@yourdomain.com;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=http://localhost;LoginPrompt=Auto"
     }
   }
   ```

4. Build and run the sample:
   ```
   dotnet run
   ```

## What this sample does

When you run the sample, you will see output similar to:

```
Connected to Dataverse.

Setup complete - no pre-existing entities required.

Creating account entity named 'Fourth Coffee'...
Created account entity named Fourth Coffee.

Retrieving account...
Retrieved entity:
  Name: Fourth Coffee
  Owner ID: <guid>

Updating account...
Updated entity:
  Address 1 Postal Code: 98052
  Address 2 Postal Code: null (cleared)
  Revenue: $5,000,000
  Credit On Hold: false

Sample completed successfully.

Press any key to undo environment data changes.

Cleaning up entities...
Deleted account entity.
Cleanup complete.
```

## Demonstrates

- Using late-bound `Entity` class for dynamic entity operations
- Creating entities with `IOrganizationService.Create()`
- Retrieving entities with `IOrganizationService.Retrieve()` and `ColumnSet`
- Updating entities with proper update pattern
- Deleting entities with `IOrganizationService.Delete()`
- Working with `Money` type for currency fields
- Clearing attribute values by setting to null
- Tracking created entities for cleanup

## See Also

[Create entities using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-create)
[Retrieve an entity using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-retrieve)
[Update and delete entities using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-update-delete)
[Late-bound and early-bound programming using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/early-bound-programming)
