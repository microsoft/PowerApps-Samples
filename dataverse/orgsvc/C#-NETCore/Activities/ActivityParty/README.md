---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to create a letter activity."
---

# Create a letter activity

Learn how to create a letter activity addressed to multiple contacts.

Related article: [Activity tables](https://learn.microsoft.com/power-apps/developer/data-platform/activity-entities)

## About the sample code

|Sample|Description|Build target|
|---|---|---|
|ActivityParty|Demonstrates creating a letter activity.|.NET 8|

The code samples demonstrates how to create a letter activity. Specifically, the samples demonstrates how to:

1. Connect to Dataverse using a [connection string](https://learn.microsoft.com/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect) that defines required connection information
1. Create a [letter activity](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/letter) to send to multiple [contacts](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/contact)
1. Use the Dataverse [organization service context](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/organizationservicecontext) to process the data changes
1. Use [early-bound](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/early-bound-programming#early-bound) entity types

The early-bound entity files in the project's *DataModel* folder were generated using the following PAC CLI command:
`pac modelbuilder build`. More information: [pac modelbuilder](https://learn.microsoft.com/en-us/power-platform/developer/cli/reference/modelbuilder)

## Sample code design

This table highlights the program's class members and how they are used.

|Member name|Purpose|
|--|--|
|Program()|Class constructor that reads the connection string value from the app settings JSON file. The code supports a global app settings file referred to by an environment variable. Otherwise, the app settings file included with the project is used.|
|Main()|Executes the Setup(), Run(), and Cleanup() methods (in that order).|
|Setup()|Pre-creates any entity or other data required by the Run() method. This code is not the primary focus of the code sample.|
|Run()|Performs the primary operations being demonstrated by the code sample.|
|Cleanup()|Deletes all created entities and frees any other resources allocated during the execution of the program.|
|entityStore| Dictionary that stores information about all entities created in Dataverse by Setup() and Run(). Used by Cleanup() to delete any created entities.|

## How to build and run the code sample(s)

1. Clone the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Locate the sample folder.
1. Open the solution file (*.sln) in Visual Studio.
1. Edit the project's appsettings.json file and set the `Url`value as appropriate for your Dataverse test environment.
1. Build and run the project [F5].
1. You will be prompted in a browser window for account logon credentials to the target environment.

## Expected program output

For a successful run, the program's console output should look similar to the following example.
Otherwise, any errors or exceptions will be displayed.

```console
Press any key to undo environment data changes.
```
