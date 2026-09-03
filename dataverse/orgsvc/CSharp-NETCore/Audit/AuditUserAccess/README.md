---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates how to enable and retrieve audit records for user access"
---

# AuditUserAccess

Demonstrates how to enable user access auditing and retrieve audit records that track when users access Microsoft Dataverse.

## What this sample does

This sample shows how to:
- Enable user access auditing at the organization level
- Configure auditing settings for tracking user activity
- Create and update records to generate user activity
- Query and retrieve user access audit records
- Display audit information including user actions and timestamps

User access auditing tracks when users log in to Dataverse and access the system through web services or the web interface.

## How this sample works

### Setup

The sample performs the following setup operations:
1. Records the sample start time for filtering audit records
2. Retrieves the current organization auditing and user access auditing settings
3. Enables organization-level auditing if not already enabled
4. Enables user access auditing if not already enabled
5. Displays the configured user access auditing interval
6. Enables auditing on the account entity
7. Creates a new account record
8. Updates the account to generate audit activity

### Run

The main demonstration includes:
1. Building a query to retrieve user access audit records
2. Filtering for specific audit actions:
   - UserAccessAuditStarted (64)
   - UserAccessAuditStopped (65)
   - UserAccessviaWebServices (66)
   - UserAccessviaWeb (67)
3. Filtering records to only those created during the sample execution
4. Optionally filtering to only show the current user's access records
5. Displaying audit details including:
   - Action type
   - User who performed the action
   - Timestamp
   - Operation type
   - Related record information

**Important Note**: User access audit records may not appear immediately. Dataverse typically batches and creates these records based on the configured auditing interval (often several hours).

### Cleanup

The cleanup process:
1. Restores the original organization auditing setting
2. Restores the original user access auditing setting
3. Disables auditing on the account entity
4. Deletes the created account record

## Demonstrates

This sample demonstrates the following SDK messages and patterns:
- **WhoAmIRequest/Response**: Get the current user and organization IDs
- **QueryExpression**: Building complex queries with multiple criteria
- **FilterExpression**: Filtering audit records by action type and time
- **ConditionOperator.In**: Querying for multiple specific values
- **RetrieveEntityRequest/Response**: Retrieve entity metadata
- **UpdateEntityRequest**: Enable/disable entity-level auditing
- **BooleanManagedProperty**: Set managed properties like IsAuditEnabled
- Working with audit action codes (64-67 for user access)
- Filtering audit records by creation date

## Sample Output

```
Connected to Dataverse.

Enabling auditing on the organization and for user access...
Enabled auditing for the organization and for user access.
Auditing interval is set to 4 hours.
Creating an account...
Updating the account...
Setup complete. Account created and updated to generate audit records.

Retrieving user access audit records...

No user access audit records found for this session.
Note: User access audit records may take time to appear based on the
configured auditing interval (typically several hours).

User access audit retrieval complete.
Restoring audit settings...
Reverted organization and user access auditing to their previous values.
Deleting 1 created record(s)...
Records deleted.

Press any key to exit.
```

**Note**: If user access audit records exist, they would display like:
```
Retrieved 2 audit record(s):

  Action: User Access via Web Services,
  User: System Administrator,
  Created On: 2/6/2026 11:55:00 AM,
  Operation: Access
  Related Record: System Administrator

  Action: User Access Audit Started,
  User: System Administrator,
  Created On: 2/6/2026 11:50:00 AM,
  Operation: Access
```

## See also

[Auditing overview](https://learn.microsoft.com/power-apps/developer/data-platform/auditing-overview)
[Configure auditing](https://learn.microsoft.com/power-apps/developer/data-platform/auditing/configure)
[User access auditing](https://learn.microsoft.com/power-apps/developer/data-platform/auditing/configure#user-access-auditing)
