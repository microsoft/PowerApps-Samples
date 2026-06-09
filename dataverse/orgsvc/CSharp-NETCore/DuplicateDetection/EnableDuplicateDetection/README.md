---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates how to enable duplicate detection for the organization and entities"
---

# EnableDuplicateDetection

Demonstrates how to enable duplicate detection for the organization and entities

## What this sample does

This sample shows how to:
- Enable duplicate detection at the organization level
- Enable duplicate detection for a specific entity (account)
- Publish all duplicate detection rules for an entity
- Create duplicate records
- Retrieve duplicate records using RetrieveDuplicatesRequest

Duplicate detection must be enabled at both the organization level and the entity level before it can detect duplicates. This sample demonstrates the complete setup process.

## How this sample works

### Setup

The setup process:
1. **Enables organization-level duplicate detection**:
   - Retrieves the organization record
   - Sets four duplicate detection flags:
     - isduplicatedetectionenabled (general duplicate detection)
     - isduplicatedetectionenabledforimport (during import)
     - isduplicatedetectionenabledforofflinesync (during offline sync)
     - isduplicatedetectionenabledforonlinecreateupdate (during online CRUD)

2. **Enables entity-level duplicate detection**:
   - Retrieves entity metadata for the account entity using RetrieveEntityRequest
   - Sets IsDuplicateDetectionEnabled managed property to true
   - Updates the entity metadata using UpdateEntityRequest
   - Publishes the entity changes using PublishXmlRequest

3. **Publishes existing duplicate rules**:
   - Queries for all duplicate rules for the account entity
   - Publishes each rule using PublishDuplicateRuleRequest
   - Waits for all async publishing jobs to complete
   - Polls asyncoperation table to track job status

### Run

The main demonstration:
1. Creates two duplicate account records with the same name ("Microsoft")
2. Uses RetrieveDuplicatesRequest to find duplicates:
   - Creates a BusinessEntity with the name to check
   - Specifies the matching entity name
   - Provides paging information
3. Displays all found duplicate records with their names and IDs

**Note**: Duplicate detection may take a moment to process after enabling, so immediate results may not appear.

### Cleanup

The cleanup process:
1. Restores original organization auditing settings if they were changed
2. Deletes all created account records

## Demonstrates

This sample demonstrates:
- **Organization entity updates**: Modifying organization-level settings
- **RetrieveEntityRequest/Response**: Getting entity metadata
- **UpdateEntityRequest**: Modifying entity metadata properties
- **BooleanManagedProperty**: Setting managed metadata properties
- **PublishXmlRequest**: Publishing entity metadata changes
- **PublishDuplicateRuleRequest/Response**: Publishing duplicate detection rules
- **Async job tracking**: Waiting for multiple async operations to complete
- **RetrieveDuplicatesRequest/Response**: Retrieving duplicate records
- **QueryByAttribute**: Finding duplicate rules by entity name
- **PagingInfo**: Controlling result set size

## Sample Output

```
Connected to Dataverse.

Enabling duplicate detection...
Enabling duplicate detection for organization: org-id-here
Organization duplicate detection enabled.
Retrieving entity metadata for account...
Enabling duplicate detection for account...
Publishing account entity...
Entity account published.
Retrieving duplicate rules for account...
Found 2 duplicate rule(s). Publishing...
  Publishing duplicate rule: rule-id-1
  Publishing duplicate rule: rule-id-2
Waiting for async operations to complete...
  Async operation completed: job-id-1
  Async operation completed: job-id-2
All duplicate rules published successfully.
Duplicate detection enabled and rules published.

Creating duplicate account records...
Created duplicate records:
  Account 1: account-id-1
  Account 2: account-id-2

Retrieving duplicate records...
Found 2 duplicate(s):
  Microsoft (ID: account-id-1)
  Microsoft (ID: account-id-2)

Duplicate detection operations complete.
Cleaning up...
Deleting 2 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Detect duplicate data](https://learn.microsoft.com/power-apps/developer/data-platform/detect-duplicate-data)
[Enable and disable duplicate detection](https://learn.microsoft.com/power-apps/developer/data-platform/enable-disable-duplicate-detection)
