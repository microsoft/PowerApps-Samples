---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates how to detect multiple duplicate records using BulkDetectDuplicatesRequest"
---

# DetectMultipleDuplicateRecords

Demonstrates how to detect multiple duplicate records using BulkDetectDuplicatesRequest

## What this sample does

This sample shows how to:
- Create duplicate account records programmatically
- Create a duplicate detection rule with multiple conditions
- Publish the duplicate detection rule
- Use BulkDetectDuplicatesRequest to detect duplicates across multiple records
- Wait for the asynchronous bulk detection job to complete
- Query and retrieve the detected duplicate records

The BulkDetectDuplicatesRequest is used to detect duplicates for all records of a specified entity type, running as an asynchronous background job.

## How this sample works

### Setup

The setup process:
1. Creates 2 duplicate account records with the same name ("Contoso, Ltd") and website ("http://www.contoso.com/")
2. Creates 1 non-duplicate account with a different name ("Contoso Pharmaceuticals") but the same website
3. Creates a duplicate detection rule programmatically that checks for duplicates based on both account name and website URL
4. Adds two rule conditions:
   - Exact match on "name" attribute
   - Exact match on "websiteurl" attribute
5. Publishes the duplicate detection rule using PublishDuplicateRuleRequest
6. Waits for the publish operation to complete (async job tracking)

### Run

The main demonstration:
1. Creates a BulkDetectDuplicatesRequest with:
   - JobName for identifying the operation
   - QueryExpression to specify which records to check (all accounts)
   - Empty recurrence pattern (one-time execution)
2. Executes the request, which returns a JobId for the async operation
3. Waits for the bulk detection job to complete (polls asyncoperation table)
4. Queries the duplicaterecord table to retrieve all detected duplicates
5. Verifies that the expected duplicate accounts were detected
6. Displays results including duplicate record IDs

### Cleanup

The cleanup process:
1. Deletes all created records in reverse order to handle dependencies:
   - Async operation record
   - Duplicate rule conditions
   - Duplicate detection rule
   - Account records
2. Handles errors gracefully if deletion fails

## Demonstrates

This sample demonstrates:
- **BulkDetectDuplicatesRequest/Response**: Initiating bulk duplicate detection
- **PublishDuplicateRuleRequest/Response**: Publishing duplicate detection rules programmatically
- **Programmatic rule creation**: Creating duplicate rules and conditions via SDK
- **Async job tracking**: Polling asyncoperation table for job completion
- **QueryByAttribute**: Querying duplicaterecord table by asyncoperationid
- **Multiple rule conditions**: Creating rules with AND logic across multiple attributes
- **EntityReference tracking**: Managing created entities for cleanup

## Sample Output

```
Connected to Dataverse.

Creating duplicate account records...
  Created 2 duplicate accounts (Name=Contoso, Ltd, Website=http://www.contoso.com/)
  Created non-duplicate account (Name=Contoso Pharmaceuticals, Website=http://www.contoso.com/)

Creating duplicate detection rule...
  Rule created: a1234567-89ab-cdef-0123-456789abcdef
  Rule conditions created
Publishing duplicate detection rule...
  Waiting for rule to publish...
  Rule published successfully

Creating BulkDetectDuplicatesRequest...
Executing BulkDetectDuplicatesRequest...
  Job ID: b2345678-90cd-ef01-2345-6789abcdef01
  Waiting for duplicate detection job to complete...
Querying for detected duplicates...
  Found 2 duplicate record(s):
    Base Record ID: c3456789-01de-f012-3456-789abcdef012
    Base Record ID: d4567890-12ef-0123-4567-89abcdef0123
  All expected duplicate accounts were detected successfully!

Bulk duplicate detection complete.
Cleaning up...
Deleting 6 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Detect duplicate data](https://learn.microsoft.com/power-apps/developer/data-platform/detect-duplicate-data)
[Run duplicate detection](https://learn.microsoft.com/power-apps/developer/data-platform/run-duplicate-detection)
