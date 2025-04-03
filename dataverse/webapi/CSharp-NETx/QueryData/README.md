---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to use capabilities to query data using the Dataverse Web API."
---
# Web API Query Data sample

This .NET 6.0 sample demonstrates how to use capabilities to query data using the Dataverse Web API.

This sample uses the common helper code in the [WebAPIService](../WebAPIService) class library project.

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with privileges to perform data operations

## How to run the sample

1. Clone or download the [PowerApps-Samples](../../../../../PowerApps-Samples) repository.
1. Open the [QueryData.sln](QueryData.sln) file using Visual Studio 2022.
1. Edit the [appsettings.json](../appsettings.json) file to set the following property values:

   | Property | Instructions |
   |----------|--------------|
   | `Url` | The URL for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. Learn how to find your URL in [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources). |
   | `UserPrincipalName` | Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access your environment. |
   | `Password` | Replace the placeholder `yourPassword` value with the password you use. |

1. Save the `appsettings.json` file.
1. Press `F5` to run the sample.

## Demonstrates

This sample has 11 sections:

1. [Create Records to query](#section-1-create-records-to-query)
1. [Selecting specific properties](#section-2-select-specific-properties)
1. [Using query functions](#section-3-use-query-functions)
1. [Ordering and aliases](#section-4-ordering-and-aliases)
1. [Limit and count results](#section-5-limit-and-count-results)
1. [Pagination](#section-6-pagination)
1. [Expanding results](#section-7-expand-results)
1. [Aggregate results](#section-8-aggregate-results)
1. [FetchXML queries](#section-9-fetchxml-queries)
1. [Using predefined queries](#section-10-use-predefined-queries)
1. [Delete sample records](#section-11-delete-sample-records)

### Section 1: Create records to query

Operations: Create one `account` record with nine related `contact` records. Each `contact` has three related `task` records.

This created data is used in the rest of this sample.

### Section 2: Select specific properties

Operations:

- Use `$select` against a contact entity to get the properties you want.
- Include annotations to provide access to formatted values with the `@OData.Community.Display.V1.FormattedValue` annotation.

### Section 3: Use query functions

Operations:

- Use standard query functions (`contains`, `endswith`, `startswith`) to filter results.
- Use Dataverse query functions (`LastXhours`, `Last7Days`, `Today`, `Between`, `In`).
- Use filter operators and logical operators (`eq`, `ne`, `gt`, `and`, `or`).
- Set precedence using parenthesis `((criteria1) and (criteria2)) or (criteria3)`.

### Section 4: Ordering and aliases

Operations:

- Use `$orderby`.
- Use parameterized aliases (`?@p1=fullname`) with `$filter` and `$orderby`.

### Section 5: Limit and count results

Operations:

- Limit results using `$top`.
- Get a count value using `$count`.

### Section 6: Pagination

Operations:

- Use the `Prefer: odata.maxpagesize` request header to limit the number of rows returned.
- Use the URL returned with the `@odata.nextLink` annotation to retrieve the next set of records.

### Section 7: Expand results

Operations:

- `$expand` with single-valued navigation properties.
- `$expand` with partner property.
- `$expand` with collection-valued navigation properties.
- `$expand` with multiple navigation property types in a single request.
- Nested `$expand`.
- Nested `$expand` having both single-valued and collection-valued navigation properties.

### Section 8: Aggregate results

Operations: Use `$apply=aggregate` with `average`, `sum`, `min`, & `max`.

### Section 9: FetchXML queries

Operations:

- Send requests using fetchXml using `?fetchXml=`.
- Simple paging using `page` and `count` attributes.

### Section 10: Use predefined queries

Operations:

- Use `{entitysetname}?savedQuery={savedqueryid}` to return the results of a saved query (system view).
- Use `{entitysetname}?userQuery={userquery}` to return the results of a user query (saved view).

### Section 11: Delete sample records

Operations: A reference to each record created in this sample was added to a list as it was created. In this sample the records are deleted using a `$batch` operation using the WebAPIService [BatchRequest class](../WebAPIService/Batch/BatchRequest.cs).

## Clean up

By default, this sample deletes all the records created in it.

If you want to view created records after the sample is completed, change the `deleteCreatedRecords` variable to `false` and you're  prompted to delete the records, if desired.
