# Web API CDSWebApiService Query Data Sample

This Sample demonstrates how to define an perform queries using the CDSWebApiService class with the Common Data Service Web API.

The CDSWebApiService class provides helper methods that are aligned to the Http methods used, with some variations for different use cases.

The CDSWebApiService class also demonstrates best practices for managing an HttpClient and properly handling Service Protection Limit 429 errors that client applications should expect.

## How to run the sample

Wee [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## Demonstrates

This class demonstrates how to use the CDSWebApiService class methods to perform Queries:

Selecting specific properties
Using Query Functions
Ordering and alias
Limit results
Expanding results
Aggregate results
FetchXml queries
Using predefined queries

## Clean up

By default this sample will delete all the records created in it.

If you want to view created records after the sample is completed, change the `deleteCreatedRecords` variable to `false`.
