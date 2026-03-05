# Late-bound Entity Operations

This sample demonstrates Create, Retrieve, Update, and Delete (CRUD) operations using the late-bound `Entity` class in Microsoft Dataverse.

## What is Late-bound Programming?

Late-bound programming uses the generic `Entity` class with string-based attribute access instead of strongly-typed, generated entity classes (early-bound). This approach provides:

- **Flexibility**: No need to regenerate entity classes when metadata changes
- **Dynamic scenarios**: Works well with custom entities or dynamic attribute access
- **Simplicity**: No code generation step required

**Trade-offs**: Late-bound programming lacks compile-time type checking and IntelliSense support that early-bound provides.

## What This Sample Does

This sample demonstrates:

1. **Creating an entity** - Instantiates a late-bound `Entity` object and sets attributes using string indexer syntax
2. **Retrieving an entity** - Uses `ColumnSet` to specify which attributes to retrieve
3. **Updating attributes** - Shows how to work with different data types:
   - String values (`address1_postalcode`)
   - Null values (`address2_postalcode`)
   - Money type (`revenue`)
   - Boolean type (`creditonhold`)
4. **Deleting an entity** - Removes the created record during cleanup

## How to Run This Sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository
2. Navigate to the sample directory:
   ```
   cd dataverse/orgsvc/CSharp-NETCore/CRUD/LateBoundEntityOperations
   ```
3. Update the connection string in `../../appsettings.json` with your Dataverse environment details:
   ```json
   {
     "ConnectionStrings": {
       "default": "AuthType=OAuth;Url=https://yourorg.crm.dynamics.com;Username=youruser@yourdomain.com;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=http://localhost;LoginPrompt=Auto"
     }
   }
   ```
4. Build and run the sample:
   ```bash
   dotnet build
   dotnet run
   ```

## Sample Output

```
Connected to Dataverse.

=== Late-bound Entity Operations Sample ===

--- Setup ---
Created account 'Fourth Coffee' with ID: <guid>

--- Demonstrate ---
Retrieved account:
  Name: Fourth Coffee
  Owner ID: <guid>

Updating account attributes...
  Set address1_postalcode to '98052'
  Set address2_postalcode to null
  Set revenue to $5,000,000
  Set creditonhold to false
Account updated successfully.

Verifying updates...
Updated account values:
  Name: Fourth Coffee
  Postal Code 1: 98052
  Postal Code 2: (null)
  Revenue: 5000000
  Credit On Hold: False

Press any key to undo environment data changes.

--- Cleanup ---
Deleted account with ID: <guid>
Cleanup completed.
Program complete. Press any key to exit.
```

## Key Concepts

### Late-bound Entity Creation

```csharp
// Instantiate using logical name
var account = new Entity("account");

// Set attributes using string indexer
account["name"] = "Fourth Coffee";
account["address1_postalcode"] = "98052";

// Create in Dataverse
Guid id = serviceClient.Create(account);
```

### Working with Different Data Types

```csharp
// String
account["name"] = "Fourth Coffee";

// Money (special Dataverse type)
account["revenue"] = new Money(5000000);

// Boolean
account["creditonhold"] = false;

// Null (removes attribute value)
account["address2_postalcode"] = null;
```

### Retrieving with ColumnSet

```csharp
// Specify only needed columns for efficiency
var columnSet = new ColumnSet("name", "ownerid", "revenue");
var account = serviceClient.Retrieve("account", accountId, columnSet);

// Access attributes
string name = account.GetAttributeValue<string>("name");
Money revenue = account.GetAttributeValue<Money>("revenue");
```

## See Also

- [Use the Entity class for create, update and delete](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-create)
- [Retrieve a table row using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-retrieve)
- [Update and delete table rows using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-update-delete)
- [Late-bound and early-bound programming using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/early-bound-programming)
- [Sample: Early-bound entity operations](../EarlyBoundEntityOperations/) - Compare with early-bound approach

## Requirements

- .NET 6.0 SDK or later
- Access to a Microsoft Dataverse environment
- Appropriate permissions to create and delete account records
