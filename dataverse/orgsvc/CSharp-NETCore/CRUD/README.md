# CRUD

Samples demonstrating Create, Read, Update, and Delete operations in Dataverse.

These samples show fundamental entity operations including working with late-bound and early-bound entities, handling related records, using upsert operations, optimistic concurrency, and record initialization patterns.

More information: [Entity operations using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations)

## Samples

|Sample folder|Description|Build target|
|---|---|---|
|CRUD-Dynamic-Entity|Basic CRUD operations using late-bound entities|.NET 6|
|CreateUpdateRecordsWithRelatedRecords|Create and update records with related entities|.NET 6|
|EarlyBoundEntityOperations|CRUD operations using early-bound entity classes|.NET 6|
|LateBoundEntityOperations|CRUD operations using late-bound Entity class|.NET 6|
|InitializeRecordFromExisting|Initialize new records from existing records|.NET 6|
|InsertRecordUsingUpsert|Insert or update records using Upsert|.NET 6|
|OptimisticConcurrency|Handle concurrent updates with RowVersion|.NET 6|

## Prerequisites

- Visual Studio 2022 or later
- .NET 6.0 SDK or later
- Access to a Dataverse environment

## How to run samples

1. Clone the PowerApps-Samples repository
2. Navigate to `dataverse/orgsvc/CSharp-NETCore/CRUD/`
3. Open the desired sample folder
4. Edit the `appsettings.json` file (located in the CRUD folder) with your environment connection details:
   ```json
   {
     "ConnectionStrings": {
       "default": "AuthType=OAuth;Url=https://yourorg.crm.dynamics.com;Username=youruser@yourdomain.com;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=http://localhost;LoginPrompt=Auto"
     }
   }
   ```
5. Build and run the sample:
   ```bash
   cd SampleFolder
   dotnet run
   ```

## See also

[Entity operations using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations)
[Create entities using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-create)
[Retrieve an entity using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-retrieve)
[Update and delete entities using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-update-delete)
