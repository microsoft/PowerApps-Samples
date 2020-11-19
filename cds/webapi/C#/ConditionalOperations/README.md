# Conditional message operations using the Web API

This sample shows how to perform conditional message operations when retrieving, updating, and deleting an account of the Microsoft Dataverse. The sample solution includes a shared project named CDSWebApiService that provides helper methods for message sending and receiving, performance enhancements, and error processing.

More information: [CDSWebApiService class](https://docs.microsoft.com/powerapps/developer/common-data-service/webapi/samples/cdswebapiservice), [Web API Conditional Operations Sample](https://docs.microsoft.com/powerapps/developer/common-data-service/webapi/web-api-conditional-operations-sample)

## How to run this sample

1. Download or clone the sample so that you have a local copy.
1. In the sample's folder, locate the solution file and load it into Visual Studio 2017 or later.
1. In **Solution Explorer** locate and edit the project's App.config file and set appropriate values for the Dataverse environment you intend to use: connectionString `Url`, `UserPrincipalName`, and `Password`. You only need to perform this step once for all samples that share this file.
1. Press F5 to build and run the program in debug mode

## Demonstrates

This sample uses a conditional (If-Match, If-None-Match) message header along with the ETag value of an account record when retrieving, updating, or deleting an account.

## Clean up

Prior to termination, the program will display a console prompt asking if you want any created entity records deleted. If you want to view created records after the sample terminates, answer 'n' or 'no' and press \[Enter\].
