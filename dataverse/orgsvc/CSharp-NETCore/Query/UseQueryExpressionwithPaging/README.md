---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates paging with QueryExpression using PagingInfo"
---

# UseQueryExpressionwithPaging

Demonstrates paging with QueryExpression using PagingInfo and paging cookies

## What this sample does

This sample shows how to:
- Use PagingInfo with QueryExpression to retrieve records in pages
- Use paging cookies to navigate through result pages
- Handle the MoreRecords flag to determine when to stop paging
- Display results from multiple pages

Paging is essential when working with large datasets to improve performance and avoid timeouts.

## How this sample works

### Setup

The setup process:
1. Creates 1 parent account ("Root Test Account")
2. Creates 10 child accounts linked to the parent account

### Run

The main demonstration:
1. Creates a QueryExpression to retrieve child accounts
2. Sets PageInfo with Count=3 (page size)
3. Loops through pages:
   - Retrieves records for current page
   - Displays records with page separators
   - Checks MoreRecords flag
   - Updates PagingCookie for next page
   - Increments PageNumber
4. Continues until all records are retrieved

### Cleanup

The cleanup process deletes all created accounts (parent and children).

## Demonstrates

This sample demonstrates:
- **QueryExpression**: Building queries with filters and ordering
- **PagingInfo**: Configuring page size and page number
- **PagingCookie**: Using cookies to navigate through pages
- **MoreRecords**: Determining if more pages exist
- **EntityCollection**: Working with paged result sets

## Sample Output

```
Connected to Dataverse.

Creating sample account records...
Created 1 parent and 10 child accounts.

Retrieving sample account records in pages...

#	Account Name			Email Address
1.	Child Test Account 1	child1@root.com
2.	Child Test Account 10	child10@root.com
3.	Child Test Account 2	child2@root.com

****************
Page number 1
****************
#	Account Name			Email Address
4.	Child Test Account 3	child3@root.com
5.	Child Test Account 4	child4@root.com
6.	Child Test Account 5	child5@root.com

****************
Page number 2
****************
#	Account Name			Email Address
7.	Child Test Account 6	child6@root.com
8.	Child Test Account 7	child7@root.com
9.	Child Test Account 8	child8@root.com

****************
Page number 3
****************
#	Account Name			Email Address
10.	Child Test Account 9	child9@root.com

Cleaning up...
Deleting 11 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Page large result sets with QueryExpression](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/page-large-result-sets-with-queryexpression)
[Build queries with QueryExpression](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/build-queries-with-queryexpression)
