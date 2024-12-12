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

- Microsoft Visual Studio 2022.
- Access to Dataverse with privileges to perform data operations.

## How to run the sample

1. Clone or download the [PowerApps-Samples](../../../../../PowerApps-Samples) repository.
1. Open the [QueryData.sln](QueryData.sln) file using Visual Studio 2022.
1. Edit the [appsettings.json](../appsettings.json) file to set the following property values:

   |Property|Instructions  |
   |---------|---------|
   |`Url`|The Url for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. See [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources) to find this. |
   |`UserPrincipalName`|Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access the environment.|
   |`Password`|Replace the placeholder `yourPassword` value with the password you use.|

1. Save the `appsettings.json` file
1. Press F5 to run the sample.

## Demonstrates

This sample has 11 regions:

### Section 0: Create Records to query

Operations: Create 1 `account` record with 9 related `contact` records. Each `contact` has 3 related `task` records.

This is the data that will be used in this sample.

### Section 1 Selecting specific properties

Operations:

- Using `$select `against a contact entity to get the properties you want.
- Including annotations provides access to formatted values with the `@OData.Community.Display.V1.FormattedValue` annotation

### Section 2 Using query functions

Operations:

- Using standard query functions (`contains`, `endswith`, `startswith`) to filter results.
- Using Dataverse query functions (`LastXhours`, `Last7Days`, `Today`, `Between`, `In`)
- Using filter operators and logical operators (`eq`, `ne`, `gt`, `and`, `or`)
- Set precedence using parenthesis `((criteria1) and (criteria2)) or (criteria3)`

### Section 3 Ordering and aliases

Operations:

- Using `$orderby`
- Using parameterized aliases (`?@p1=fullname`) with `$filter` and `$orderby`

### Section 4 Limit and count results

Operations:

- Limiting results using `$top`.
- Get a count value using `$count`.

### Section 5 Pagination

Operations:

- Use the `Prefer: odata.maxpagesize` request header to limit the number of rows returned.
- Use the url returned with the `@odata.nextLink` annotation to retrieve the next set of records.

### Section 6 Expanding results

Operations:

- `$expand` with single-valued navigation properties.
- `$expand` with partner property.
- `$expand` with collection-valued navigation properties.
- `$expand` with multiple navigation property types in a single request.
- Nested `$expand`.
- Nested `$expand` having both single-valued and collection-valued navigation properties

### Section 7 Aggregate results

Operations: Using `$apply=aggregate` with `average`, `sum`, `min`, & `max`.

### Section 8 FetchXML queries

Operations: 

 - Sending requests using fetchXml using `?fetchXml=`
 - Simple paging using `page` and `count` attributes

### Section 9 Using predefined queries

Operations:

- Using `{entitysetname}?savedQuery={savedqueryid}` to return the results of a saved query (system view)
- Using `{entitysetname}?userQuery={userquery}` to return the results of a user query (saved view)

### Section 10: Delete sample records

Operations: A reference to each record created in this sample was added to a list as it was created. In this sample the records are deleted using a `$batch` operation using the WebAPIService [BatchRequest class](../WebAPIService/Batch/BatchRequest.cs).

## Clean up

By default this sample will delete all the records created in it.

If you want to view created records after the sample is completed, change the `deleteCreatedRecords` variable to `false` and you will be prompted to decide if you want to delete the records.
