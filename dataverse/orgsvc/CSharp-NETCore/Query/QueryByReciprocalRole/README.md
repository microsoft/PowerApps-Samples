---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates querying connection roles by reciprocal role"
---

# QueryByReciprocalRole

Demonstrates querying connection roles by reciprocal role

## What this sample does

This sample shows how to:
- Create connection roles with reciprocal associations
- Query for connection roles using the connectionroleassociation relationship
- Use QueryExpression with LinkEntity to filter by associated connection role
- Work with connection role object type codes

Connection roles define the types of relationships that can exist between records in Dataverse. Reciprocal roles define the inverse relationship (e.g., "Manager" and "Report").

## How this sample works

### Setup

The setup process:
1. Creates a primary connection role ("Example Primary Connection Role")
2. Creates a connection role object type code associating the primary role with the Account entity
3. Creates a reciprocal connection role ("Example Reciprocal Connection Role")
4. Creates a connection role object type code associating the reciprocal role with the Account entity
5. Associates the primary and reciprocal roles using the connectionroleassociation relationship

### Run

The main demonstration:
1. Creates a QueryExpression for the "connectionrole" entity
2. Adds a LinkEntity to join with the "connectionroleassociation" entity
3. Filters by the reciprocal connection role ID in the association
4. Executes RetrieveMultiple to retrieve all connection roles that have the specified reciprocal role
5. Displays the retrieved connection role IDs and names

### Cleanup

The cleanup process deletes all created connection roles and connection role object type codes in reverse order to handle dependencies.

## Demonstrates

This sample demonstrates:
- **Connection Roles**: Creating and configuring connection roles
- **Connection Role Associations**: Associating connection roles as reciprocals
- **AssociateRequest**: Using the Associate message to create many-to-many relationships
- **QueryExpression with LinkEntity**: Querying across related entities
- **Connection Role Object Type Codes**: Defining which entity types a role applies to
- **Entity-based syntax**: Using late-bound Entity objects with string-based attribute access

## Sample Output

```
Connected to Dataverse.

Creating sample data...
Created primary connection role: Example Primary Connection Role
Created Connection Role Object Type Code for Account on primary role.
Created reciprocal connection role: Example Reciprocal Connection Role
Created Connection Role Object Type Code for Account on reciprocal role.
Associated primary and reciprocal connection roles.
Setup complete.

Querying for connection roles by reciprocal role...

Retrieved 1 connection role(s) with reciprocal role association.

Connection Role ID: <guid>
Connection Role Name: Example Primary Connection Role

Cleaning up...
Deleting 4 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Connection roles](https://learn.microsoft.com/power-apps/developer/data-platform/connection-entities)
[Build queries with QueryExpression](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/build-queries-with-queryexpression)
[Join tables using QueryExpression](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/join-tables-using-queryexpression)
[Use the Associate message](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-associate-disassociate)
