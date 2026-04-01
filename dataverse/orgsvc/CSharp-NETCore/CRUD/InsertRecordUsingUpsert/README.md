# Insert or update a record using Upsert (.NET 6.0)

This sample demonstrates how to insert or update records using the [UpsertRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.upsertrequest) message with alternate keys.

## Key Concepts

- **UpsertRequest**: Performs an insert-or-update operation in a single request
- **Alternate Keys**: Uses alternate keys (instead of primary GUID) to identify records
- **Late-bound Entity**: Creates entities without requiring generated early-bound classes
- **XML Processing**: Reads and processes product data from XML files

## How to run this sample

1. Download or clone this repository
2. Open the `InsertRecordUsingUpsert.csproj` file in Visual Studio 2022 or later
3. Update the connection string in `../appsettings.json` with your Dataverse environment URL and credentials
4. Press F5 to run the sample

Alternatively, use the .NET CLI:

```bash
cd dataverse/orgsvc/CSharp-NETCore/CRUD/InsertRecordUsingUpsert
dotnet build
dotnet run
```

## Prerequisites

This sample requires the **UpsertSample** managed solution to be installed in your environment. The solution creates:

- `sample_product` table
- `sample_productcode` alternate key on the Code field

The solution file (`UpsertSample_1_0_0_0_managed.zip`) can be found in the legacy sample directory or imported from the original Power Apps documentation.

## What this sample does

The `UpsertRequest` message performs an update if a record with the specified alternate key exists, or creates a new record if it doesn't exist. This eliminates the need to check for record existence before deciding whether to create or update.

## How this sample works

### Setup

1. Verifies the alternate key indexes are active and ready for use
2. The alternate key creation is asynchronous, so the sample waits if the index is still being built

### Demonstrate

1. **First upsert operation** (`newsampleproduct.xml`):
   - Processes 13 product records
   - Since these products don't exist yet, all 13 are created as new records
   - `UpsertResponse.RecordCreated` returns `true` for each

2. **Second upsert operation** (`updatedsampleproduct.xml`):
   - Processes 6 product records with the same product codes as before
   - Since these products already exist (matched by alternate key), they are updated
   - `UpsertResponse.RecordCreated` returns `false` for each
   - Notice the "Updated" suffix in the product names

### Key Code Pattern

```csharp
// Create entity using alternate key constructor
var product = new Entity("sample_product", "sample_productcode", productCode)
{
    ["sample_name"] = productName,
    ["sample_category"] = productCategory,
    ["sample_make"] = productMake
};

// Execute UpsertRequest
var upsertRequest = new UpsertRequest { Target = product };
var upsertResponse = (UpsertResponse)service.Execute(upsertRequest);

// Check if record was created or updated
if (upsertResponse.RecordCreated)
    Console.WriteLine("New record created!");
else
    Console.WriteLine("Existing record updated!");
```

### Cleanup

Deletes all created product records (if user confirms).

## Sample Output

```
Connected to Dataverse.

=== Setup ===

Verifying alternate key is active...
Alternate key is active and ready.
Setup complete.

=== Demonstrate UpsertRequest ===

Processing newsampleproduct.xml...
  Created: Nam01 (Code: Cam01)
  Created: Nam02 (Code: Ody01)
  Created: Nam03 (Code: Civ01)
  ...

Summary: 13 created, 0 updated

Processing updatedsampleproduct.xml...
  Updated: Nam01Updated (Code: Cam01)
  Updated: Nam02Updated (Code: Ody01)
  Updated: Nam03Updated (Code: Civ01)
  ...

Summary: 0 created, 6 updated

UpsertRequest demonstration complete.

=== Cleanup ===

Deleting 13 created record(s)...
Records deleted.

Press any key to exit.
```

## Key Features

- **Modern .NET 6.0** implementation using `Microsoft.PowerPlatform.Dataverse.Client`
- **Setup/Run/Cleanup** pattern for clear code organization
- **Entity tracking** for proper cleanup
- **Error handling** with detailed exception messages
- **Alternate key validation** to ensure indexes are ready before operations
- **XML processing** to demonstrate real-world data import scenarios

## See Also

- [UpsertRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.upsertrequest)
- [Use alternate keys](https://learn.microsoft.com/power-apps/developer/data-platform/use-alternate-key-create-record)
- [Define alternate keys for a table](https://learn.microsoft.com/power-apps/developer/data-platform/define-alternate-keys-entity)
- [Work with data using code](https://learn.microsoft.com/power-apps/developer/data-platform/work-with-data)
