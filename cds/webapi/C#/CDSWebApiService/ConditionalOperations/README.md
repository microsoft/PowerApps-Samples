# Use of conditional Web API message operations

This sample shows how to perform conditional message operations when accessing records of the Common Data Service. 

## How to run this sample

To run the sample:
1. Download or clone the sample so that you have a local copy.
2. Open the  solution file (CDSWebApiService.sln) in Visual Studio and restore the NuGet packages.
3. In **Solution Explorer**, edit the App.config file located under the **Solution items** node. Set the Url, UserPrincipalName, and Password values for your environment
4. In **Solution Explorer**, right-click the **ConditionalOperations** node and select **Set as Startup Project**. 
5. Press F5 to run the sample.

## What this sample does

This sample uses a conditional message header when retrieving, updating, and deleting an account. Descriptive output is sent to the console. 

### Setup

This sample doesn't require any special setup other than what is described above.


### Demonstrate

This sample uses a conditional (If-Match, If-Non-Match) message header along with the ETag value of an account record when retrieving, updating, or deleting an account.

This sample also makes use of the CDSWebApiService wrapper class for HTTP message operations and error handling.

### Clean up

Prior to program exit, this sample displays an option to delete any created records. The deletion is optional in case you want to examine the entities and data created by the sample.
You can manually delete the records to achieve the same result.