---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to use capabilities to efficiently query schema data and maintain a cache of the data as it changes over time."
---
# RetrieveMetadataChanges Sample

This sample demonstrates the use of the [RetrieveMetadataChanges Function](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/retrievemetadatachanges) using the Dataverse Web API.

This sample depends on the [WebAPIService Class Library project](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/samples/webapiservice) which provides common helper code for all the Dataverse C# Web API Samples.

## Instructions

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.

1. Locate the /dataverse/webapi/C#-NETx/RetrieveMetadataChanges/ folder.

1. Open the *RetrieveMetadataChanges.sln* solution file in Visual Studio 2022.

1. Edit the `appsettings.json` file to set the following property values:

   |Property|Instructions  |
   |---------|---------|
   |`Url`|The Url for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. See [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources) to find this. |
   |`UserPrincipalName`|Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access the environment.|
   |`Password`|Replace the placeholder `yourPassword` value with the password you use.|

1. Save the `appsettings.json` file
1. Press F5 to run the sample.

## Demonstrates

This sample shows how to retrieve schema definitions for a specific set of column definitions and save them (in memory) to represent a cache. 

Then it creates a new column, retrieves the data for only that new column, which it adds to the cache.

Then it deletes the column, retrieves data about deleted items and uses it to remove the deleted column definition from the cache.

This sample has 6 sections:

### Define query

Define a query using [PowerApps.Samples.Metadata.Types.EntityQueryExpression](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/C%23-NETx/WebAPIService/Metadata/Types/EntityQueryExpression.cs) that will return all the Picklist choice columns from the contact table.

### Initialize cache

1. Create an instance of [PowerApps.Samples.Metadata.Messages.RetrieveMetadataChangesRequest](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/C%23-NETx/WebAPIService/Metadata/Messages/RetrieveMetadataChangesRequest.cs) with the `Query` parameter set to the query.
1. Send the request using [Service.SendAsync](https://github.com/microsoft/PowerApps-Samples/blob/d1762853517c2df1f9c33d5ecbae1fe36b71d496/dataverse/webapi/C%23-NETx/WebAPIService/Service.cs#L172)<[RetrieveMetadataChangesResponse](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/C%23-NETx/WebAPIService/Metadata/Messages/RetrieveMetadataChangesResponse.cs)>.
1. Cache the `RetrieveMetadataChangesResponse.EntityMetadata` value.
1. Save the `RetrieveMetadataChangesResponse.ServerVersionStamp` value for use in the next request.
1. Write a list of all the current columns in the cache.

### Add choice column

Create a new choice column by creating a new `PicklistAttributeMetadata` instance in the contact table.

### Detect added column

1. Create a new instance of [PowerApps.Samples.Metadata.Messages.RetrieveMetadataChangesRequest](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/C%23-NETx/WebAPIService/Metadata/Messages/RetrieveMetadataChangesRequest.cs) with the `Query` parameter set to the original query.
1. Set the `RetrieveMetadataChangesRequest.ClientVersionStamp` with the value previously returned from the first request.
1. Send the request using [Service.SendAsync](https://github.com/microsoft/PowerApps-Samples/blob/d1762853517c2df1f9c33d5ecbae1fe36b71d496/dataverse/webapi/C%23-NETx/WebAPIService/Service.cs#L172)<[RetrieveMetadataChangesResponse](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/C%23-NETx/WebAPIService/Metadata/Messages/RetrieveMetadataChangesResponse.cs)>.
1. Verify that only one new column definition was returned to represent the choice column that was created.
1. Save the `RetrieveMetadataChangesResponse.ServerVersionStamp` value for use in the next request.
1. Add that choice column data to the cache.

### Delete choice column

Delete the choice column created earlier.

### Detect deleted column

1. Create a new instance of [PowerApps.Samples.Metadata.Messages.RetrieveMetadataChangesRequest](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/C%23-NETx/WebAPIService/Metadata/Messages/RetrieveMetadataChangesRequest.cs) with the `Query` parameter set to the original query.
1. Set the `RetrieveMetadataChangesRequest.ClientVersionStamp` with the value previously returned from the second request.
1. Set the `RetrieveMetadataChangesRequest.DeletedMetadataFilters` to `DeletedMetadataFilters.Attribute` because we are looking for deleted column definitions.
1. Send the request using [Service.SendAsync](https://github.com/microsoft/PowerApps-Samples/blob/d1762853517c2df1f9c33d5ecbae1fe36b71d496/dataverse/webapi/C%23-NETx/WebAPIService/Service.cs#L172)<[RetrieveMetadataChangesResponse](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/C%23-NETx/WebAPIService/Metadata/Messages/RetrieveMetadataChangesResponse.cs)>.
1. Find the Id of the deleted choice column in the `RetrieveMetadataChangesResponse.DeletedMetadata`, using `DeletedMetadataFilters.Attribute` as an index value for the collection.
1. Remove the column definition from the cache.
1. Write a list of all the current columns in the cache.

## Clean up

No clean up is required because all data created by this sample was deleted.

