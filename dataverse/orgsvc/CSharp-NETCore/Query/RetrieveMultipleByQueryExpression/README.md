---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates querying data using QueryExpression with linked entities"
---

# RetrieveMultipleByQueryExpression

Demonstrates querying data using QueryExpression with linked entities

## What this sample does

This sample shows how to:
- Use QueryExpression to build complex queries
- Add LinkEntity to join related entities
- Use entity aliases to identify columns from related entities
- Retrieve and access aliased values from linked entities

QueryExpression provides a powerful object model for building complex queries with joins, filters, and sorting.

## How this sample works

### Setup

The setup process:
1. Creates a contact record (ContactFirstName ContactLastName)
2. Creates three account records, each with the contact as primary contact

### Run

The main demonstration:
1. Creates a QueryExpression for the "account" entity
2. Adds a LinkEntity to join with the "contact" entity via primarycontactid
3. Specifies "primarycontact" as the entity alias for the linked entity
4. Retrieves columns from both account and linked contact
5. Executes RetrieveMultiple to get all matching records
6. Displays account names and primary contact information using AliasedValue

### Cleanup

The cleanup process deletes all created accounts and contacts.

## Demonstrates

This sample demonstrates:
- **QueryExpression**: Building complex queries using the object model
- **LinkEntity**: Joining related entities in queries
- **Entity aliases**: Identifying columns from linked entities
- **AliasedValue**: Accessing columns from linked entities in results
- **JoinOperator**: Using inner joins in queries

## Sample Output

```
Connected to Dataverse.

Creating sample data...
Setup complete.

Entering: RetrieveMultipleWithRelatedEntityColumns

Retrieved 3 entities

Account name: Test Account1
Primary contact first name: ContactFirstName
Primary contact last name: ContactLastName

Account name: Test Account2
Primary contact first name: ContactFirstName
Primary contact last name: ContactLastName

Account name: Test Account3
Primary contact first name: ContactFirstName
Primary contact last name: ContactLastName

Cleaning up...
Deleting 4 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Build queries with QueryExpression](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/build-queries-with-queryexpression)
[Join tables using QueryExpression](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/join-tables-using-queryexpression)
[Query data using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-query-data)
