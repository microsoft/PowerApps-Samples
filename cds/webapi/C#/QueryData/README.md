# Query entity data using the Web API

This sample demonstrates how to define and perform data queries on Microsoft Dataverse entity instances using the Web API. The sample solution includes a shared project named CDSWebApiService that provides helper methods for message sending and receiving, performance enhancements, and error processing.

More information: [CDSWebApiService class](https://docs.microsoft.com/powerapps/developer/common-data-service/webapi/samples/cdswebapiservice), [Web API Query Data Sample](https://docs.microsoft.com/powerapps/developer/common-data-service/webapi/web-api-query-data-sample)

## How to run the sample

1. Download or clone the sample so that you have a local copy.

1. In the sample's folder, locate the solution file and load it into Visual Studio 2017 or later.

1. In **Solution Explorer** locate and edit the project's App.config file and set appropriate values for the Dataverse environment you intend to use: connectionString `Url`, `UserPrincipalName`, and `Password`. You only need to perform this step once for all samples that share this file.

1. Press F5 to build and run the program in debug mode.

## Demonstrates

This sample demonstrates how to perform the following queries on entity instance data:

- Selecting specific properties
- Using Query Functions
- Ordering and alias
- Limit results
- Expanding results
- Aggregate results
- FetchXml queries
- Using predefined queries

## Clean up

Prior to termination, the program will display a console prompt asking if you want any created entity records deleted. If you want to view created records after the sample terminates, answer 'n' or 'no' and press \[Enter\].
