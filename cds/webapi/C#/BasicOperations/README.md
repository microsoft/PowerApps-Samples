# Web API CDSWebApiService Basic Operations Sample

This Sample demonstrates how to perform basic CRUD (Create, Retrieve, Update, and Delete) and associative operations using the CDSWebApiService class with the Microsoft Dataverse Web API.

The CDSWebApiService class provides helper methods that are aligned to the  Http methods used, with some variations for different use cases. It helps to reduce duplication of code found in the other BasicOperations sample.

The CDSWebApiService class also demonstrates best practices for managing an HttpClient and properly handling Service Protection Limit 429 errors that client applications should expect.

## How to run the sample

Wee [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## Demonstrates

This class demonstrates how to use the CDSWebApiService class methods to perform basic operations:

- Create entities using the PostCreate method
- Update entities using the Patch method
- Retrieve entities using the Get method
- Set the value of a specific property using the Put method
- Associating entities on create using the PostCreate method
- Creating multiple related entities with the PostCreate method
- Associating and disassociating entities with the Post and Delete method
- Deleting records using the Delete method

## Clean up

By default this sample will delete all the records created in it.

If you want to view created records after the sample is completed, change the `deleteCreatedRecords` variable to `false`.
