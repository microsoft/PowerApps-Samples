---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates querying connections for a specific record"
---

# QueryByRecord

Demonstrates querying connections for a specific record

## What this sample does

This sample shows how to:
- Query connection records using QueryExpression to find all connections a specific entity record is part of
- Create connection roles and associated object type codes
- Create connections between different entity records
- Use QueryExpression with FilterExpression to filter by record ID

Connection records link two records together in Dataverse. This sample demonstrates how to query connections to find all related records, regardless of whether the target record is in the record1id or record2id position.

## How this sample works

### Setup

The setup process:
1. Creates a connection role with "Business" category
2. Creates connection role object type codes for both Account and Contact entities
3. Creates two account records ("Example Account 1" and "Example Account 2")
4. Creates a contact record ("Example Contact")
5. Creates a connection between Account 1 and the Contact
6. Creates a connection between the Contact and Account 2

### Run

The main demonstration:
1. Retrieves the contact ID from the entity store
2. Creates a QueryExpression to query the "connection" entity
3. Adds a filter condition on "record1id" to find connections where the contact is record1
4. Executes RetrieveMultiple to retrieve matching connection records
5. Displays the count and IDs of all connections found

**Important**: Dataverse automatically creates reciprocal connection records, so querying against just record1id will find all connections the entity is part of, whether it's in the record1 or record2 position.

### Cleanup

The cleanup process deletes all created records in reverse order to handle dependencies:
- Connection records
- Contact record
- Account records
- Connection role object type codes
- Connection role

## Demonstrates

This sample demonstrates:
- **QueryExpression**: Building queries with criteria and column sets
- **FilterExpression**: Adding filter conditions to queries
- **ConditionExpression**: Specifying attribute-based filter criteria
- **Connection Entity**: Working with connection records to link entities
- **ConnectionRole**: Creating and using connection roles
- **EntityReference**: Using late-bound entity references
- **RetrieveMultiple**: Executing queries to retrieve multiple records

## Sample Output

```
Connected to Dataverse.

Setting up sample data...
Created connection role: Example Connection Role
Created a related Connection Role Object Type Code record for Account.
Created a related Connection Role Object Type Code record for Contact.
Created Example Account 1.
Created Example Account 2.
Created contact: Example Contact.
Created a connection between account 1 and the contact.
Created a connection between the contact and account 2.
Setup complete.

Query Connections By Record
===============================
Retrieved 2 connection instances for the contact.
  Connection ID: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
  Connection ID: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
===============================
Cleaning up...
Deleting 8 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Connection entities](https://learn.microsoft.com/power-apps/developer/data-platform/connection-entities)
[Build queries with QueryExpression](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/build-queries-with-queryexpression)
[Query data using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-query-data)
[Create connections](https://learn.microsoft.com/power-apps/developer/data-platform/configure-connection-roles)
