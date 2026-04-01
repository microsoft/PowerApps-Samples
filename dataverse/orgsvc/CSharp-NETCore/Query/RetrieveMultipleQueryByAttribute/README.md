---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates querying data using QueryByAttribute"
---

# RetrieveMultipleQueryByAttribute

Demonstrates querying data using QueryByAttribute

## What this sample does

This sample shows how to:
- Use QueryByAttribute to query records based on attribute values
- Specify columns to return using ColumnSet
- Filter records by attribute name and value pairs
- Iterate through query results

QueryByAttribute provides a simple way to query records when you need to filter by specific attribute values without building complex QueryExpression criteria.

## How this sample works

### Setup

The setup process:
1. Creates account "A. Datum Corporation" with address in Colorado
2. Creates account "Adventure Works Cycle" with address in Redmond, Washington

### Run

The main demonstration:
1. Creates a QueryByAttribute for the "account" entity
2. Specifies columns to return: name, address1_city, emailaddress1
3. Adds filter criteria: address1_city = "Redmond"
4. Executes RetrieveMultiple to get matching records
5. Displays account information for all matching records

### Cleanup

The cleanup process deletes all created accounts.

## Demonstrates

This sample demonstrates:
- **QueryByAttribute**: Simple attribute-based query construction
- **ColumnSet**: Specifying which columns to retrieve
- **RetrieveMultiple**: Executing queries to retrieve multiple records
- **EntityCollection**: Working with query result collections

## Sample Output

```
Connected to Dataverse.

Creating sample accounts...
Setup complete.

Query Using QueryByAttribute
===============================
Name: Adventure Works Cycle
Address: Redmond
E-mail: contactus@adventureworkscycle.com
===============================
Cleaning up...
Deleting 2 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Query data using QueryByAttribute](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/use-querybyattribute-class)
[Build queries with QueryExpression](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/build-queries-with-queryexpression)
[Query data using the SDK for .NET](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/entity-operations-query-data)
