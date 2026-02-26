# Optimistic Concurrency Sample

This sample demonstrates how to use optimistic concurrency with update and delete operations in Dataverse using the RowVersion property.

## What this sample does

This sample shows how to:

1. Create an account record with an initial credit limit
2. Retrieve the account and capture its RowVersion
3. Update the account using `ConcurrencyBehavior.IfRowVersionMatches` to ensure the update only succeeds if the RowVersion hasn't changed
4. Delete the account using optimistic concurrency to ensure the delete only succeeds if the RowVersion matches

## How this sample works

### Setup

1. Creates an account record named "Fourth Coffee" with a credit limit of $50,000
2. Stores the account reference in the entityStore for use in Run and Cleanup

### Demonstrate

1. Retrieves the account record and displays its current RowVersion
2. Creates an in-memory Entity object with the same LogicalName, Id, and RowVersion
3. Updates the credit limit to $1,000,000 using an UpdateRequest with `ConcurrencyBehavior.IfRowVersionMatches`
4. Retrieves the account again to show the new RowVersion after the update
5. Stores the updated RowVersion for use in Cleanup

### Cleanup

1. Prompts the user to confirm deletion
2. Attempts to delete the account using a DeleteRequest with `ConcurrencyBehavior.IfRowVersionMatches`
3. If the delete fails (e.g., due to concurrent modification), falls back to a regular delete

## Key Concepts

### Optimistic Concurrency

Optimistic concurrency is a technique that allows multiple users to access the same data without locking it. Instead, the system checks whether the data has changed since it was retrieved before allowing an update or delete operation to proceed.

### RowVersion

The `RowVersion` property is a unique identifier that changes every time a record is updated. By including the RowVersion when performing update or delete operations, you can ensure that your operation only succeeds if no one else has modified the record since you retrieved it.

### ConcurrencyBehavior

The `ConcurrencyBehavior` enumeration has three values:

- **Default** (0): Normal behavior - operation proceeds regardless of RowVersion
- **IfRowVersionMatches** (1): Operation only succeeds if the RowVersion matches
- **AlwaysOverwrite** (2): Operation always succeeds, overwriting any changes

## How to run this sample

1. Update the connection string in `appsettings.json` with your Dataverse environment details:
   - Replace `yourorg.crm.dynamics.com` with your organization URL
   - Replace `youruser@yourdomain.com` with your username

2. Build and run the sample:
   ```bash
   cd CSharp-NETCore/CRUD/OptimisticConcurrency
   dotnet build
   dotnet run
   ```

3. You will be prompted to sign in via your default browser

4. The sample will:
   - Create a test account
   - Demonstrate optimistic concurrency with updates
   - Ask if you want to delete the test data

## See also

- [Optimistic concurrency](https://learn.microsoft.com/power-apps/developer/data-platform/optimistic-concurrency)
- [Use UpdateRequest and DeleteRequest](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-update-delete#check-for-duplicate-records)
- [Microsoft.Xrm.Sdk.Messages Namespace](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages)
