---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample code shows how to use efficiently retrieve schema data and maintain a cache the RetrieveMetadataChangesRequest"
---
# RetrieveMetadataChanges Sample

This sample demonstrates the use of the [RetrieveMetadataChangesRequest](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrievemetadatachangesrequest) 
and [RetrieveMetadataChangesResponse](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrievemetadatachangesresponse) classes to create a 
simple client-side persistent cache of Dataverse schema data.

## Instructions

See the instructions in the [Schema samples README](../README.md) to run this sample.

## Demonstrates

This sample shows how to retrieve schema definitions for a specific set of column definitions and save them (in memory) to represent a cache. 

Then it creates a new column, retrieves the data for only that new column, which it adds to the cache.

Then it deletes the column, retrieves data about deleted items and uses it to remove the deleted column definition from the cache.

This sample has 6 sections:

### Define query

Define a query using `EntityQueryExpression` that will return all the Picklist choice columns from the contact table.

### Initialize cache

1. Create an instance of `RetrieveMetadataChangesRequest` with the `Query` parameter set to the query.
1. Send the request using `IOrganizationService.Execute`.
1. Cache the `RetrieveMetadataChangesResponse.EntityMetadata` value.
1. Save the `RetrieveMetadataChangesResponse.ServerVersionStamp` value for use in the next request.
1. Write a list of all the current columns in the cache.

### Add choice column

Create a new choice column by creating a new `PicklistAttributeMetadata` instance in the contact table.

### Detect added column

1. Create an instance of `RetrieveMetadataChangesRequest` with the `Query` parameter set to the original query.
1. Set the `RetrieveMetadataChangesRequest.ClientVersionStamp` with the value previously returned from the first request.
1. Send the request using `IOrganizationService.Execute`.
1. Verify that only one new column definition was returned to represent the choice column that was created.
1. Save the `RetrieveMetadataChangesResponse.ServerVersionStamp` value for use in the next request.
1. Add that choice column data to the cache.

### Delete choice column

Delete the choice column created earlier.

### Detect deleted column

1. Create an instance of `RetrieveMetadataChangesRequest` with the `Query` parameter set to the original query.
1. Set the `RetrieveMetadataChangesRequest.ClientVersionStamp` with the value previously returned from the second request.
1. Set the `RetrieveMetadataChangesRequest.DeletedMetadataFilters` to `DeletedMetadataFilters.Attribute` because we are looking for deleted column definitions.
1. Send the request using `IOrganizationService.Execute`.
1. Find the Id of the deleted choice column in the `RetrieveMetadataChangesResponse.DeletedMetadata`, using `DeletedMetadataFilters.Attribute` as an index value for the collection.
1. Remove the column definition from the cache.
1. Write a list of all the current columns in the cache.

## Clean up

No clean up is required because all data created by this sample was deleted.
