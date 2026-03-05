---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates paging with FetchXML using page and paging-cookie attributes"
---

# UseFetchXMLWithPaging

Demonstrates paging with FetchXML using page and paging-cookie attributes to retrieve records in pages

## What this sample does

This sample shows how to:
- Use FetchXML with paging attributes (page, count, paging-cookie)
- Execute FetchXML queries using RetrieveMultipleRequest
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
1. Creates a FetchXML query to retrieve child accounts
2. Defines paging parameters (fetchCount=3, pageNumber=1, pagingCookie=null)
3. Loops through pages:
   - Calls CreateXml() to inject paging attributes into FetchXML
   - Executes RetrieveMultipleRequest with FetchExpression
   - Displays records with page separators
   - Checks MoreRecords flag
   - Updates pagingCookie from EntityCollection.PagingCookie
   - Increments pageNumber
4. Continues until all records are retrieved (MoreRecords = false)

### Cleanup

The cleanup process deletes all created accounts (parent and children) in reverse order.

## Demonstrates

This sample demonstrates:
- **FetchXML**: Building XML-based queries with filters and ordering
- **Paging Attributes**: Using page, count, and paging-cookie attributes
- **XML Manipulation**: Dynamically adding attributes to FetchXML
- **RetrieveMultipleRequest**: Executing FetchXML with request/response pattern
- **PagingCookie**: Using cookies to maintain paging state
- **MoreRecords**: Determining if more pages exist
- **EntityCollection**: Working with paged result sets

## Sample Output

```
Connected to Dataverse.

Creating sample account records...
Created 1 parent and 10 child accounts.

Retrieving data in pages

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

[Page large result sets with FetchXML](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/page-large-result-sets-with-fetchxml)
[Build queries with FetchXML](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/use-fetchxml-construct-query)
[FetchXML reference](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/reference/index)
