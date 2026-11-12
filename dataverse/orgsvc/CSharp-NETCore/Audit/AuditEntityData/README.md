---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates how to enable auditing on an entity and retrieve the change history"
---

# AuditEntityData

Demonstrates how to enable auditing on an entity and retrieve the change history of records and attributes in Microsoft Dataverse.

## What this sample does

This sample shows how to:
- Enable auditing at the organization level
- Enable auditing for a specific entity (account)
- Create and update records to generate audit history
- Retrieve and display record change history
- Retrieve and display attribute change history
- Retrieve detailed audit information for specific audit records

## How this sample works

### Setup

The sample performs the following setup operations:
1. Retrieves the current organization auditing setting and caches it for restoration
2. Enables auditing at the organization level
3. Enables auditing on the account entity
4. Creates a new account record
5. Updates the account with additional attributes (account number, category code, phone number)

### Run

The main demonstration includes:
1. Retrieving the complete change history for the account record
2. Displaying audit details including who made changes and when
3. Updating the account's telephone number
4. Retrieving the attribute-specific change history for the telephone1 field
5. Displaying old and new values for changed attributes
6. Retrieving detailed audit information for a specific audit record

### Cleanup

The cleanup process:
1. Restores the original organization auditing setting
2. Restores the original account entity auditing setting
3. Deletes the created account record

## Demonstrates

This sample demonstrates the following SDK messages and patterns:
- **WhoAmIRequest/Response**: Get the current organization ID
- **RetrieveEntityRequest/Response**: Retrieve entity metadata to check audit settings
- **UpdateEntityRequest**: Enable/disable entity-level auditing
- **RetrieveRecordChangeHistoryRequest/Response**: Get complete audit history for a record
- **RetrieveAttributeChangeHistoryRequest/Response**: Get audit history for a specific attribute
- **RetrieveAuditDetailsRequest/Response**: Get detailed information about a specific audit entry
- **AuditDetail**: Base class for audit detail types
- **AttributeAuditDetail**: Contains old and new values for attribute changes
- **BooleanManagedProperty**: Used to set managed properties like IsAuditEnabled

## Sample Output

```
Connected to Dataverse.

Enabling auditing on the organization and account entities...
Creating an account...
Updating the account...
Setup complete.

Retrieving the account change history...

Audit record created on: 2/6/2026 11:50:23 AM
Entity: account, Action: Update, Operation: Update
Operation performed by System Administrator
Attribute: accountnumber, old value: (no value), new value: 1-A
Attribute: accountcategorycode, old value: (no value), new value: 1
Attribute: telephone1, old value: (no value), new value: 555-555-5555

Updating the Telephone1 field in the Account entity...
Retrieving the attribute change history for Telephone1...

Audit record created on: 2/6/2026 11:50:24 AM
Entity: account, Action: Update, Operation: Update
Operation performed by System Administrator
Attribute: telephone1, old value: 555-555-5555, new value: 123-555-5555

Retrieving audit details for an audit record...

Audit record created on: 2/6/2026 11:50:24 AM
Entity: account, Action: Update, Operation: Update
Operation performed by System Administrator
Attribute: telephone1, old value: 555-555-5555, new value: 123-555-5555

Audit operations complete.
Restoring audit settings...
Audit settings restored.
Deleting 1 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Auditing overview](https://learn.microsoft.com/power-apps/developer/data-platform/auditing-overview)
[Retrieve and delete the history of audited data changes](https://learn.microsoft.com/power-apps/developer/data-platform/auditing/retrieve-audit-data)
